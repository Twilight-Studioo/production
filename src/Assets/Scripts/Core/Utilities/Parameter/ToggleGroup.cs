using System;

namespace Core.Utilities.Parameter
{
    public class ToggleGroupAttribute: Attribute
    {
        public string GroupName { get; private set; }
        public ToggleGroupAttribute(string groupName)
        {
            GroupName = groupName;
        }
    }
}