using System;

namespace Warsmiths.Common.Utils
{
    public static class MathfUtils
    {
        public static int CeilToInt(this float arg)
        {
            return (int)Math.Ceiling(arg);
        }

        public static int RoundToInt(this float arg)
        {
            return (int) Math.Round(arg);
        }
    }
}
