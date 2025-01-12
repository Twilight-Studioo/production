using System;

namespace Core.Utilities.Parameter
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ToggleGroupAttribute : Attribute
    {
        public ToggleGroupAttribute(string groupName)
        {
            GroupName = groupName;
        }

        public string GroupName { get; private set; }
    }
}