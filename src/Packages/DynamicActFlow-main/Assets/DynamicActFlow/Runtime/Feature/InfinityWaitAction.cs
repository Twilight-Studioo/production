#region

using DynamicActFlow.Runtime.Core;
using DynamicActFlow.Runtime.Core.Action;
using UnityEngine;

#endregion

namespace DynamicActFlow.Runtime.Feature
{
    [ActionTag(ActionName.InfinityWait)]
    public class InfinityWaitAction : FixedUpdatedAction
    {
        private float elapsedTime;
        [ActionParameter("Seconds", 1f)] private float Seconds { get; set; }

        public override void OnCreated()
        {
            base.OnCreated();
            Seconds = 1f;
        }

        protected override void Start()
        {
            elapsedTime = 0;
        }

        protected override void FixedUpdate()
        {
            elapsedTime += Time.fixedDeltaTime;
        }

        protected override bool CheckIfEnd() => elapsedTime >= Seconds;
    }
}