using System;

namespace Core.Utilities.Parameter
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ToggleGroupAttribute: Attribute
    {
        public string GroupName { get; private set; }
        public ToggleGroupAttribute(string groupName)
        {
            GroupName = groupName;
        }
    }
}