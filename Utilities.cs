using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NEINGames.Utilities
{

    public static class Vector2Utilities {

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
    }

    public static class EnumUtitlities
    {
        public static int GetLength(Type enumType) => Enum.GetNames(enumType).Length;
    }
}