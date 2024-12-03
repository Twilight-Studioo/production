#region

using System;

#endregion

namespace DynamicActFlow.Runtime.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class TriggerTagAttribute : Attribute
    {
        public TriggerTagAttribute(string tag)
        {
            Tag = tag;
        }

        public string Tag { get; private set; }
    }
}