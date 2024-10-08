using DynamicActFlow.Runtime.Core;
using DynamicActFlow.Runtime.Core.Action;
using UnityEngine;

namespace Feature.Common.ActFlow
{
    public delegate bool SelfCheck();
    [TriggerTag("SelfCheck")]
    public class SelfCheckTrigger: TriggerBase
    {
        [TriggerParameter("Trigger")] private SelfCheck Trigger { get; set; }
        public override void OnCreated()
        {
            Trigger = null;
        }

        public override bool IfEnd(MonoBehaviour owner)
        {
            return Trigger != null && Trigger.Invoke();
        }
    }
}