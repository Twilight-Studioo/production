#region

using DynamicActFlow.Runtime.Core.Action;

#endregion

namespace DynamicActFlow.Runtime.Core.Flow
{
    public interface IFlowBuilder
    {
        internal ActionRef GetActionRef(ActionBase action);

        internal TriggerRef GetTriggerRef(TriggerBase trigger);
    }
}