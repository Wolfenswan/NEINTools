using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NEINGames.Utilities
{

    public static class Vector2Utilities 
    {

        public static float GetDegreesBetweenVectors(Vector2 origin, Vector2 target)
        {
            var dirVector = (target - origin).normalized;
            return GetVectorDegrees(dirVector);
        }
        
        public static float GetVectorDegrees(Vector2 vector)
        {
            float angle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360;
            return angle;
        }

        public static float GetGradualDistance(Vector2 oldPos, Vector2 targetPos, float baseDistance = 5f) {
            float maxDistance = baseDistance * Time.deltaTime;
            float curDistance = Vector2.Distance(targetPos, oldPos);
            float remDistance = maxDistance - curDistance;

            if (remDistance < 0.00001)
                remDistance = maxDistance;

            return remDistance;
        }

        public static Vector2 GetOffsetPointFromAngle(Vector2 origin, float offset, float angle, bool degree=true)
        {   
            // https://math.stackexchange.com/questions/143932/calculate-point-given-x-y-angle-and-distance
            if (degree) angle = angle * Mathf.Deg2Rad;

            var x = origin.x + offset * Mathf.Cos(angle);
            var y = origin.y + offset * Mathf.Sin(angle);

            return new Vector2(x,y);
        }
        
        // UNFINISHED
        // public static Vector2 LerpWithOffset(Vector2 pStart, Vector2 pEnd, float t, float offset)
        // {   
        //     Vector2 lerp = Vector2.Lerp(pStart, pEnd, t);
        //     Vector2 perpendicular = Vector2.Perpendicular(lerp).normalized;

        //     var x = pStart.x + t*(pEnd.x-pStart.x) + offset * perpendicular.x;
        //     var y = pStart.y + t*(pEnd.y-pStart.y) + offset * perpendicular.y;
            
        //     return new Vector2(x,y);
        // }
    }

    public static class DictionaryUtilities 
    {
        public static bool CompareDictionaries(Dictionary<dynamic, dynamic> d1, Dictionary<dynamic, dynamic> d2) {
            foreach (var key in d1.Keys)
                {
                    if (!d1[key].Equals(d2[key]))
                        return false;
                }
            return true;
        }

        public static IEnumerable<TValue> RandomValues<TKey, TValue>(IDictionary<TKey, TValue> dict)
        {
            // https://stackoverflow.com/posts/1028324/revisions
            System.Random rand = new System.Random();
            List<TValue> values = Enumerable.ToList(dict.Values);
            int size = dict.Count;
            while(true)
            {
                yield return values[rand.Next(size)];
            }
        }
    }

    public static class EnumUtitlities
    {
        public static int GetLength(Type enumType) => Enum.GetNames(enumType).Length;
    }

    public static class RaycastUtilities
    {
        public static bool IsPoint2DOverElementWithTag(Vector2 position, string tag)
        //! This is expensive and can probably be made more performant
        // Adapted from http://answers.unity.com/answers/1748972/view.html
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = position;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count(obj => obj.gameObject.tag == tag) > 0;
        }
    }

    public static class CoroutineUtilities
    {
        public static IEnumerator DoOverTime(float defaultDuration, Action doWhileLooping, float timeStep = -1, Action doAfterLoop = null, Func<bool> breakCondition = null)
        //! Untested, just an idea
        {
            var elapsedTime = 0.0f;
            while (elapsedTime < defaultDuration)
            {
                yield return new WaitForEndOfFrame();
                elapsedTime += timeStep>0?timeStep:Time.deltaTime;
                doWhileLooping.Invoke();
                if (breakCondition != null)
                    if (breakCondition()) break;
            }
            doAfterLoop?.Invoke();
        }
    }
}