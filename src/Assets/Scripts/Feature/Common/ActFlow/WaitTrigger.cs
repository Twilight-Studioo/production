#region

using DynamicActFlow.Runtime.Core;
using DynamicActFlow.Runtime.Core.Action;
using UnityEngine;

#endregion

namespace Feature.Common.ActFlow
{
    [TriggerTag("WaitTrigger")]
    public class WaitTrigger : TriggerBase
    {
        private float startTimestamp;
        [TriggerParameter("Seconds")] private float Seconds { get; set; }

        public override void OnCreated()
        {
            Seconds = 1.0f;
        }

        public override void Start()
        {
            base.Start();
            startTimestamp = Time.time;
        }

        public override bool IfEnd(MonoBehaviour owner) => Time.time - startTimestamp > Seconds;
    }
}