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
            var objects = field.GetCustomAttributes(typeof(CustomDisplayAttribute), true).ToList();
            if (!objects.Any())
            {
                return string.Empty;
            }
            var first = objects.First();
            return ((CustomDisplayAttribute)first).DisplayName;
        }

        public static string MakeName(this Enum value)
        {
            var s = value.ToString();
            var res = string.Empty;
            for (var i = 0; i < s.Length; i++)
            {
                res += (char.IsUpper(s[i]) ? " " : string.Empty) + s[i];
            }

            return res;
        }
    }
}