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
    }
}