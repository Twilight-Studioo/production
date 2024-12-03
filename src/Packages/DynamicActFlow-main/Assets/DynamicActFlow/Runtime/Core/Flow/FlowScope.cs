#region

using System.Collections;
using DynamicActFlow.Runtime.Core.Action;
using UnityEngine;

#endregion

namespace DynamicActFlow.Runtime.Core.Flow
{
    public abstract class FlowScope : FlowScopeBase
    {
        [SerializeField] protected bool autoPlay;

        private IFlowBuilder builder;

        private Coroutine coroutine;

        protected void Start()
        {
            if (autoPlay)
            {
                FlowStart();
            }
        }


        protected override void FlowStart()
        {
            builder = new FlowBuilder(this);
            FlowCreate();
        }

        private void FlowCreate()
        {
            FlowStop();
            coroutine = StartCoroutine(Flow(builder));
        }

        protected override void FlowStop()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }

        protected override ActionRef Action(string actionName) => builder.Action(actionName);

        protected IEnumerator Wait(float seconds) => builder.Action(ActionName.Wait).Param("Seconds", seconds).Build();

        protected ActionRef InfinityWait(float maxTime) =>
            builder.Action(ActionName.InfinityWait).Param("Seconds", maxTime);

        protected TriggerRef Trigger(string triggerName) => builder.Trigger(triggerName);
    }
}