#region

using System;

#endregion

namespace DynamicActFlow.Runtime.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ActionTagAttribute : Attribute
    {
        public ActionTagAttribute(string tag)
        {
            Tag = tag;
        }

        public string Tag { get; private set; }
    }
}