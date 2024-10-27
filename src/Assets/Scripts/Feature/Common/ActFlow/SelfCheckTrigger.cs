#region

using DynamicActFlow.Runtime.Core;
using DynamicActFlow.Runtime.Core.Action;
using UnityEngine;

#endregion

namespace Feature.Common.ActFlow
{
    public delegate bool SelfCheck();

    [TriggerTag("SelfCheck")]
    public class SelfCheckTrigger : TriggerBase
    {
        [TriggerParameter("Trigger")] private SelfCheck Trigger { get; set; }

        public override void OnCreated()
        {
            Trigger = null;
        }

        public override bool IfEnd(MonoBehaviour owner) => Trigger != null && Trigger.Invoke();
    }
}