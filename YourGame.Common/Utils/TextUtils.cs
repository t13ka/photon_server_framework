namespace YourGame.Common.Utils
{
    public static class TextUtils
    {
        public static string SplitUpperCased(this string inputString)
        {
            var newstring = string.Empty;
            for (var i = 0; i < inputString.Length; i++)
            {
                if (char.IsUpper(inputString[i]))
                    newstring += " ";
                newstring += inputString[i].ToString();
            }
            return newstring;
        }
    }
}
