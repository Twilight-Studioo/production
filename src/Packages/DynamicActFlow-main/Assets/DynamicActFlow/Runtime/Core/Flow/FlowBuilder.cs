#region

using DynamicActFlow.Runtime.Core.Action;
using UnityEngine;

#endregion

namespace DynamicActFlow.Runtime.Core.Flow
{
    internal sealed class FlowBuilder : IFlowBuilder
    {
        private readonly MonoBehaviour owner;
        private ActionBase currentAction;

        public FlowBuilder(MonoBehaviour owner)
        {
            this.owner = owner;
        }

        ActionRef IFlowBuilder.GetActionRef(ActionBase action) => action.CreateActionRef(owner);

        TriggerRef IFlowBuilder.GetTriggerRef(TriggerBase trigger) => trigger.CreateTriggerRef(owner);
    }
}