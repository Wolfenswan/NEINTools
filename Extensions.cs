using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using NEINGames.Utilities;

// more extension ideas:
// https://gist.github.com/omgwtfgames/f917ca28581761b8100f
// https://github.com/dracolytch/DracoSoftwareExtensionsForUnity

namespace NEINGames.Extensions
{
    public static class ObjectExtensions
    {   
        #if UNITY_EDITOR // Can be a pain when compiling, especially to web
        public static string FieldsToString(this object obj)
        {
            string fieldStr = "";
            foreach (FieldInfo item in obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                fieldStr += $" {item.Name}: {item.GetValue(obj).ToString()} |";
            }
            return fieldStr;
        }
        #endif
    }

    public static class EnumExtensions
    {   
        public static T CycleValues<T>(this T src) where T : Enum
        // https://stackoverflow.com/questions/642542/how-to-get-next-or-previous-enum-value-in-c-sharp
        {
            if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

            T[] Arr = (T[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf<T>(Arr, src) + 1;
            return (Arr.Length==j) ? Arr[0] : Arr[j];            
        }
    }

    public static class CameraExtensions
    {
        public static bool IsPositionInViewport(this Camera camera, Vector3 pos)
        {
            Vector3 vpPos = camera.WorldToViewportPoint(pos);
            return ((vpPos.x >=0 && vpPos.x <= 1) && (vpPos.y >=0 && vpPos.y <= 1));
        }
    }

    public static class Collider2DExtensions
    {   
        public static bool IsTouchingObject(this Collider2D collider, GameObject obj, bool enabledOnly = false)
        {   
            Collider2D[] colliders = obj.GetComponents<Collider2D>();

            if (colliders.Length == 0)
                Debug.LogError("Collider2DExtensions.IsTouchingObject: No Colliders2D found on object: " + obj);

            foreach (var c in colliders)
            {
                if (collider.IsTouching(c) && (!enabledOnly || (enabledOnly && c.enabled)))
                    return true;
            }
            return false;
            
        }

        public static Vector2 PointToWorldPos(this EdgeCollider2D collider, int pointID)
        // Returns the world-position of a given EdgeCollider's point
        {   
            var t = collider.gameObject.transform;
            return new Vector2 (t.position.x + collider.offset.x + collider.points[pointID].x, t.position.y + collider.offset.y + collider.points[pointID].y);
        }

    }

    public static class GameObjectExtensions
    {
        public static bool IsInCameraViewport(this GameObject obj, Camera camera) => camera.IsPositionInViewport(obj.transform.position);

        // IsPlayer assumes existing of the "Player" tag. It is only a shorthand, nothing fancy here.
        public static bool IsPlayer(this GameObject obj) => obj.tag == "Player";

        public static List<GameObject> GetAllChildren(this GameObject parent)
        {   
            List<GameObject> children = new List<GameObject>();
            foreach(Transform tr in parent.transform)
            {
                children.Add(tr.gameObject);
            }

            return children;
        }

        public static void DestroyAllChildren(this GameObject parent, bool editorMode = false, List<string> exemptTags = null)
        {   
            // Due to quirks in the Transform IEnumerator, deleting the items directly from the foreach loop will produce odd results
            // as explained here: https://answers.unity.com/questions/1256205/how-to-destroy-and-remove-all-children-from-an-obj.html?childToView=1256363#comment-1256363
            // Using a temporary list to store all children and then delete them does circumvent the issue
            List<GameObject> children = parent.GetAllChildren();

            exemptTags = exemptTags ?? new List<string>();
            
            foreach (var c in children)
            {
                if (!exemptTags.Contains(c.tag))
                    if(!editorMode)
                        GameObject.Destroy(c);
                    else
                        GameObject.DestroyImmediate(c);
            }
        }

        public static void DestroyAllChildrenWithTag(this GameObject parent, string tag, bool editor = false)
        {
            List<GameObject> children = parent.GetChildrenWithTag(tag);
            foreach (var c in children)
            {
                if(!editor)
                    GameObject.Destroy(c);
                else
                    GameObject.DestroyImmediate(c);
            }
        }

        public static List<GameObject> GetChildrenWithTag (this GameObject parent, string tag)
        {
            //var t = parent.transform;
            List<GameObject> children = new List<GameObject>();
            foreach(Transform tr in parent.transform)
            {
                if(tr.tag == tag)
                {
                    children.Add(tr.gameObject);
                }
            }
            return children;
        }

        public static T GetComponentInChildWithTag<T>(this GameObject parent, string tag) where T:Component
        // https://answers.unity.com/questions/893966/how-to-find-child-with-tag.html
        {
            Transform t = parent.transform;
            foreach(Transform tr in t)
            {
                    if(tr.tag == tag)
                    {
                        return tr.GetComponent<T>();
                    }
            }
            return null;
        }

        public static Vector2 GetSpriteSize(this GameObject obj, bool applyScale = true)
        {   
            var size = obj.GetComponent<SpriteRenderer>().sprite.bounds.size;
            if (applyScale)
                {
                    size.x *= obj.transform.localScale.x;
                    size.y *= obj.transform.localScale.y;
                }
            return size;
        }

        public static void CopySortingLayerFrom(this GameObject obj, GameObject otherObj)
        {   
            if (obj.TryGetComponent(out SpriteRenderer sr) && otherObj.TryGetComponent(out SpriteRenderer templateSR))
                sr.CopySortingLayerFrom(templateSR);
            else
                Debug.LogError("GameObjectExtensions.CopySortingLayerFrom: No SpriteRenderer-Component found. Obj: " + obj.GetComponent<SpriteRenderer>() + " |  Other obj: " + otherObj.GetComponent<SpriteRenderer>());
        }
    }

    public static class SpriteRendererExtensions
    {
        public static void CopySortingLayerFrom(this SpriteRenderer spriteRenderer, SpriteRenderer otherSpriteRenderer)
        {
            spriteRenderer.sortingLayerID = otherSpriteRenderer.sortingLayerID;
            spriteRenderer.sortingOrder = otherSpriteRenderer.sortingOrder;
        }
    }

    public static class TransformExtensions
    {
        // public static Vector2 PositionWithScale(this Transform transform)
        // {   
        //     var position = transform.position;
        //     position.x *= transform.localScale.x;
        //     position.y *= transform.localScale.y;
        //     return position;
        // }
    }

    public static class Vector2Extensions
    {   
        //* NOTE: Makes more sense as a utility?
        public static Vector2 MovePast(this Vector2 start, Vector2 end, float distance) => (end-start) * distance + end;
    }

    public static class RectTransformExtensions
    {
        public static void PointFromOriginTowardsTarget(this RectTransform element, Transform origin, Transform target, float angleCorrection = 0)
        // Assumes the resting orientation of the element is towards the right. Change angleCorrection to adapt to different resting orientations.
        {
            var targetPosition = target.position;
            var basePosition = origin.position;
            basePosition.z = 0f;
            float angle = Vector2Utilities.GetDegreesBetweenVectors(basePosition, targetPosition) + angleCorrection;
            element.localEulerAngles = new Vector3(0, 0, angle);
        }
    }

    public static class AnchoredJoint2DExtensions
    {
        public static Vector2 GetWorldPosition(this AnchoredJoint2D joint)
        {   
            var tr = joint.gameObject.transform;
            var scale = joint.gameObject.transform.localScale;
            return new Vector2(tr.position.x + joint.anchor.x * scale.x, tr.position.y + joint.anchor.y * scale.y);            
        }
    }

    static class Collision2DExtensions 
    { 
        public static float GetImpactForce (this Collision2D collision) {
            // https://www.malgol.com/how-to-get-the-impact-force-of-a-collision-in-unity/
            float impulse = 0F;

            foreach (ContactPoint2D point in collision.contacts) {
                impulse += point.normalImpulse;
            }

            return impulse / Time.fixedDeltaTime;
        }
    }

    static class DictionaryExtensions
    {   
        public static string ToStringKeys(this Dictionary<dynamic, dynamic> dict)
        {
            string text = "";
            foreach (var item in dict.Keys)
            {
                text += $"{item} |";
            }
            return text;
        }

        public static string ToStringValues(this Dictionary<dynamic, dynamic> dict)
        {
            string text = "";
            foreach (var item in dict.Values)
            {
                text += $"{item} |";
            }
            return text;
        }

        public static string ToStringFull(this Dictionary<dynamic, dynamic> dict)
        {
            string text = "";
            foreach (var item in dict)
            {
                text += $"{item.Key}:{item.Value} |";
            }
            return text;
        }
    }

}