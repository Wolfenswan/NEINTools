using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using NEINGames.Extensions;

public enum RaycastDirection 
{
    RIGHT,
    LEFT,
    DOWN,
    UP,
}
public enum RequiredCollisionCount 
{
    ONE,
    FULL,
    ANY_HALF,
    FIRST_HALF,
    SECOND_HALF,
}

// RaycasterGroups sent out a number of Raycasts away from a plane indicated by an EdgeCollider2D
// They check for collisions according to the settings in the fields exposed in the editor
// They are then read by the CollisionController and passed on to whichever method or class requested updated collision states

// TODO 
// Instead of using debug rays -> OnDrawGizmos() https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnDrawGizmos.html
[RequireComponent (typeof(EdgeCollider2D))]
public class RaycasterGroup : MonoBehaviour
{
    [SerializeField] public CollisionID CollisionID;
    [SerializeField] RequiredCollisionCount _requiredCollisionCount;
    [SerializeField] LayerMask _collisionMask;
    [SerializeField][Tooltip("Orientation is relative to the orientation of the bounds and the parent object.")] RaycastDirection _direction;
    [SerializeField] float _distance = 0.15f;
    [SerializeField] int _numberOfRays = 4;
    [SerializeField][Tooltip("Ignore contacts with the gameobject this raycaster group is attached to.")] bool _filterSelf = true;
    [SerializeField] bool _showDebugRays = false;
    [SerializeField] Color _debugRayColor = Color.green;

    // The public list contains all actual collisions from the last check. It is rarely accessed from outside. A good example is clinging to ledges, which requires knowledge of the corner's position.
    public List<RaycastHit2D> LastCollisions{get; private set;} = new List<RaycastHit2D>(); // Returns all collisions since the last check
    [NonSerialized] public GameObject Parent; // Only used to filter hits with the parent gameobject

    EdgeCollider2D _bounds;
    List<RaycastHit2D> _raycasts = new List<RaycastHit2D>(); // The list is cleared and refilled each check, containing all rays casts (both hits and non-collisions). 

    void Awake() 
    {
        _bounds = GetComponent<EdgeCollider2D>();
    }

    public bool CheckCollision() 
    {
        LastCollisions.Clear();

        float debugRayDuration = 0.005f;

        Vector2 startPos = _bounds.PointToWorldPos(0);
        Vector2 endPos = _bounds.PointToWorldPos(1);

        // Calculate the required spacing between each ray
        float raySpacing;
        bool horizontalRayCast = (_direction.Equals(RaycastDirection.RIGHT) || _direction.Equals(RaycastDirection.LEFT))?true:false;

        // If the beam is sent off in any horizontal direction, the origins need to be offset on the y-axis and vice versa
        if (horizontalRayCast)
            raySpacing = (startPos.y-endPos.y) / (_numberOfRays - 1);
        else
            //raySpacing = (startPos.x-endPos.x) / (_numberOfRays - 1);
            raySpacing = (startPos.x-endPos.x) / (_numberOfRays - 1);

        // Set the Vector governing the raycast's direction
        Vector2 raycastDirection = Vector2.zero;
        switch (_direction) 
        {
            case RaycastDirection.RIGHT:
                raycastDirection = Vector2.right;
                break;
            case RaycastDirection.LEFT:
                raycastDirection = Vector2.left;
                break;
            case RaycastDirection.DOWN:
                raycastDirection = Vector2.down;
                break;
            case RaycastDirection.UP:
                raycastDirection = Vector2.up;
                break;
            default:
                Debug.LogError($"Unknown RaycastDirection: {_direction}");
                break;
        }

        // Check collision for each ray
        _raycasts.Clear();
        for (int i = 0; i < _numberOfRays; i ++) 
        {
            Vector2 origin = startPos;

            if (horizontalRayCast) // As above, for checks on the horizontal plane, the origins need to be offset on the y-axis
                origin += Vector2.down * (raySpacing * i);
            else
                origin += Vector2.left * (raySpacing * i);

            RaycastHit2D hit = Physics2D.Raycast(origin, raycastDirection, _distance, _collisionMask);
            
            if (_showDebugRays)
            {   
                var color = hit?Color.red:_debugRayColor;

                if (_filterSelf)
                    color = (hit && hit.collider.gameObject != Parent)?Color.red:_debugRayColor;

                Debug.DrawRay(origin, raycastDirection * _distance, color, debugRayDuration);
            }
            _raycasts.Add(hit);
        }

        // Copy all actual collisions to the public List
        // by copying it after the fact (instead of filling it during the loop), no partially filled List can be accessed
        if (!_filterSelf)
            LastCollisions = _raycasts.Where(c => c == true).ToList();
        else
            LastCollisions = _raycasts.Where(c => c == true && c.collider.gameObject != Parent).ToList();

        // Finally return true or false, depending on the required collision count
        // For the simple collision counts (one, half full) this method would be overkill and the collisions could simply be counted during the loop above
        // However, the special counts (e.g. lower half) need to know all collisions in their respective order
        int collisionCount = LastCollisions.Count();
        if (collisionCount == 0)
            return false;

        var collision = false;
        switch (_requiredCollisionCount) {
            case RequiredCollisionCount.ONE:
                collision = collisionCount >= 1;
                break;
            case RequiredCollisionCount.ANY_HALF:
                collision = collisionCount >= _numberOfRays/2;
                break;
            case RequiredCollisionCount.FULL:
                collision = (collisionCount == _numberOfRays);
                break;
            case RequiredCollisionCount.FIRST_HALF:
                for (var i = 0; i < _numberOfRays; i++) //* CONSIDER would make more sense to start at i = number of rays and then i-- after each step
                {
                    if (_raycasts[i] && i < _numberOfRays/2)
                        collision = true;
                    else if (_raycasts[i] && i >= _numberOfRays/2)
                        collision = false; // Return false if any of the other half raycasts collide
                }
                break;
            case RequiredCollisionCount.SECOND_HALF:
                for (var i = 0; i < _numberOfRays; i++)
                {
                    if (_raycasts[i] && i < _numberOfRays/2)
                    {
                        collision = false; // Return false if any of the first half of rays did collide
                        break;
                    } else if (_raycasts[i] && i >= _numberOfRays/2)
                        collision = true; // Return true for the lower half of the colliders
                }
                break;
            default:
                Debug.LogError($"RayCasterGroup: Unknown RequiredCollisionCount: {_requiredCollisionCount}");
                break;
        }

        return collision;
    }

    public bool IsTouching(Collider2D collider) => (LastCollisions.Count>0)?LastCollisions.FirstOrDefault(hit => hit.collider == collider):false;
}