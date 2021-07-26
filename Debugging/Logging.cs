using UnityEngine;
using System.Reflection;

namespace NEINGames.Debugging
{
    #if UNITY_EDITOR
    public static class Logging
    {
        // TODO Generic Error & Info functions to improve on default unity logging.
        public static void ListFields(string intro, dynamic obj)
        {
            string debugString = $"{intro}";

            foreach (FieldInfo item in obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                debugString += $" {item.Name}: {item.GetValue(obj).ToString()} |";
            }
            debugString += $" @T:{Time.time}";
            Debug.Log(debugString);
        }

        public static void ListDictionary(string intro, dynamic dict)
        { // TODO as extension?
            string debugString = $"{intro}";
            foreach (var item in dict)
            {
                debugString += $"{item.Key}: {item.Value} |";
            }
            debugString += $"@T:{Time.time}";
            Debug.Log(debugString);
        }
    }
    #endif
}