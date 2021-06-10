using System;

namespace NEINGames.Collections
{
    [Serializable]
    public struct RangeInt
    {
        public RangeInt(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public int Min;
        public int Max;
        public int RandomIntInclusive{get => UnityEngine.Random.Range(Min, Max+1);}
        public int RandomIntExclusive{get => UnityEngine.Random.Range(Min, Max);}
    }
}