using UnityEngine;
using System;

namespace NEINGames.Singleton
{
    public abstract class Singleton<T> : MonoBehaviour where T:MonoBehaviour
    {
        // Not thread-safe but adequate for normal use
        // See https://csharpindepth.com/Articles/Singleton for more information
        private static T _instance = null;
        public static T Instance{get=>_instance;}

        protected void InitializeSingleton(T instance) 
        {
            if (_instance == null)
            {
                _instance = instance;
                DontDestroyOnLoad(instance.gameObject);
            } else
            {
                Debug.LogError($"Trying to initialize over existing Singleton: {Instance} @ {Time.time}.");
            }
        }
    }
}