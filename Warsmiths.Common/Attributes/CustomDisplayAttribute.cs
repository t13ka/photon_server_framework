using System;

namespace Warsmiths.Common.Attributes
{
    public class CustomDisplayAttribute : Attribute
    {
        public string DisplayName { get; private set; }

        public CustomDisplayAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }
}