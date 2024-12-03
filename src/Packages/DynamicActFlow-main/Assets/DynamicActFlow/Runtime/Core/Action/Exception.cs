#region

using System;

#endregion

namespace DynamicActFlow.Runtime.Core.Action
{
    public class NotSupportedParameterException : Exception
    {
        public NotSupportedParameterException(string message) : base(message)
        {
        }
    }
}