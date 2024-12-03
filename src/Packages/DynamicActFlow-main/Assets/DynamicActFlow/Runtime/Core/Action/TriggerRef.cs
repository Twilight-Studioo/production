#region

using UnityEngine;

#endregion

namespace DynamicActFlow.Runtime.Core.Action
{
    public class TriggerRef : Ref
    {
        private readonly MonoBehaviour owner;
        private readonly TriggerBase trigger;

        internal TriggerRef(TriggerBase trigger, MonoBehaviour owner)
        {
            this.trigger = trigger;
            this.owner = owner;
        }


        internal override void SetParam(string key, object param)
        {
            trigger.SetProperty(key, param);
        }

        public TriggerBase Build() => trigger;
    }
}