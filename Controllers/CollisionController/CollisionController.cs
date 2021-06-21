using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CollisionController : MonoBehaviour 
{
    [SerializeField] bool _debugLog = false;
    List<RaycasterGroup> _raycasterGroups = new List<RaycasterGroup> {};

    Dictionary<CollisionID,bool> _collisionDict = new Dictionary<CollisionID,bool>(){};

    public GameObject AttachedTo {get => transform.parent.gameObject;}
    public bool Initialized{get; private set;} = false;

    void Awake() 
    {       
            foreach(Transform child in transform)
            {
                if (child.GetComponent<RaycasterGroup>() != null && child.gameObject.activeSelf) 
                {   
                    RaycasterGroup raycasterGroup = child.GetComponent<RaycasterGroup>();
                    _collisionDict.Add(raycasterGroup.CollisionID, false);
                    raycasterGroup.Parent = AttachedTo;
                    _raycasterGroups.Add(raycasterGroup);
                }
            }

            Initialized = true;
    }

    // Update Collisions is the core method that the owning object should call each Update() to update their respective collision values
    public Dictionary<CollisionID,bool> UpdateCollisions() 
    {
        foreach (var rcGroup in _raycasterGroups)
        {   
            bool doesCollide = rcGroup.CheckCollision();
            _collisionDict[rcGroup.CollisionID] = doesCollide;
        }

        if (_debugLog) 
        {
            string debugString = "Current collisions: ";
            foreach(var item in _collisionDict)
            {
                debugString += $"{item.Key}: {item.Value} |";
            }
            Debug.Log(debugString);
        }

        return _collisionDict;
    }

    RaycasterGroup GetRaycasterGroup(CollisionID collisionType) => _raycasterGroups.Single(rcgroup => rcgroup.CollisionID == collisionType);
    List<RaycasterGroup> GetRaycasterGroups(CollisionID collisionType) => _raycasterGroups.FindAll(rcgroup => rcgroup.CollisionID == collisionType);

    public List<Vector2> GetLastCollisionPoints(CollisionID type)
    {
        var raycaster = GetRaycasterGroup(type); // Assumes there's exactly one of the type. Might need adaption in the future.
        List<Vector2> hits = new List<Vector2>();
        foreach (var item in raycaster.LastCollisions)
        {
            hits.Add(item.point);
        }
        return hits;
    }

    public List<GameObject> GetLastCollisionHits(CollisionID type)
    {
        RaycasterGroup raycaster = GetRaycasterGroup(type); // Assumes there's exactly one of the type. Might need adaption in the future.
        List<GameObject> hits = new List<GameObject>();
        foreach (var item in raycaster.LastCollisions)
        {
            hits.Add(item.collider.gameObject);
        }
        return hits;
    }

    // Check whether the Raycaster-Group with the given collisionType(s) touches a specific collider
    public bool IsTouchingSpecific(Collider2D collider, CollisionID collisionType) => GetRaycasterGroup(collisionType).IsTouching(collider);
    public bool IsTouchingSpecific(Collider2D collider, CollisionID[] collisionTypes, bool any = true) 
    {   
        var hits = 0;
        foreach (var ct in collisionTypes)
        {
            if (GetRaycasterGroup(ct).IsTouching(collider))
                {
                    if (any) return true;
                    else hits ++;

                    if (!any && hits == collisionTypes.Length) return true;
                }
        }
        return false;
    }
}