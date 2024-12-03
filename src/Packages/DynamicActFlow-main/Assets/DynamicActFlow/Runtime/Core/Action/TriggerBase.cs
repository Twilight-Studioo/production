#region

using UnityEngine;

#endregion

namespace DynamicActFlow.Runtime.Core.Action
{
    public abstract class TriggerBase : IFlowObject
    {
        public abstract void OnCreated();

        public virtual void Start()
        {
        }

        public abstract bool IfEnd(MonoBehaviour owner);
    }
}