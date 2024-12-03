#region

using System;

#endregion

namespace DynamicActFlow.Runtime.Core
{
    public abstract class ParameterBaseAttribute : Attribute
    {
        public string Tag { get; protected set; }

        public object DefaultValue { get; protected set; }
    }
}