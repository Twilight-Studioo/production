#region

using System;

#endregion

namespace DynamicActFlow.Runtime.Core
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class ActionParameterAttribute : ParameterBaseAttribute
    {
        public ActionParameterAttribute(string tag, object defaultValue = default)
        {
            Tag = tag;

            DefaultValue = defaultValue;
        }
    }
}