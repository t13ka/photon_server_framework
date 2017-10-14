namespace YourGame.Common.Attributes
{
    using System;

    public class CustomDisplayAttribute : Attribute
    {
        public string DisplayName { get; private set; }

        public CustomDisplayAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }
}