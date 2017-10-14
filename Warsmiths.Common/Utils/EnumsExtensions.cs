
// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable ForCanBeConvertedToForeach

namespace YourGame.Common.Utils
{
    using System;
    using System.Linq;

    using YourGame.Common.Attributes;

    public static class EnumsExtensions
    {
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribs = field.GetCustomAttributes(typeof(CustomDisplayAttribute), true).ToList();
            if (!attribs.Any()) return string.Empty;
            var first = attribs.First();
            return ((CustomDisplayAttribute)first).DisplayName;
        }

        public static string MakeName(this Enum value)
        {
            var s = value.ToString();
            var res = "";
            for (var i = 0; i < s.Length; i++)
            {
                res += (char.IsUpper(s[i]) ? " " : "") + s[i];
            }
            return res;
        }
    }
}