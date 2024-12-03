#region

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

namespace DynamicActFlow.Runtime.Core.Action
{
    public abstract class ActionBase : IFlowObject
    {
        /// <summary>
        ///     Represents the owner of an Action.
        /// </summary>
        protected MonoBehaviour Owner;

        private List<TriggerBase> Trigger { get; set; } = new();

        private float? Timeout { get; set; }

        private float ElapsedTime { get; set; }

        public virtual void OnCreated()
        {
        }

        internal void SetTrigger(TriggerBase trigger)
        {
            Trigger.Add(trigger);
        }

        internal void SetTriggers(TriggerBase[] triggers)
        {
            Trigger = new(triggers);
        }

        internal void SetTimeout(float timeout)
        {
            Timeout = timeout;
        }

        internal void StartTrigger()
        {
            Trigger.ForEach(trigger => trigger.Start());
        }

        protected bool IfEndWithTrigger()
        {
            return (Trigger != null && Trigger.Any(trigger => trigger.IfEnd(Owner))) ||
                   (Timeout.HasValue && Time.time - ElapsedTime > Timeout.Value);
        }

        protected abstract IEnumerator OnAction();

        public IEnumerator ExecuteActionCoroutine(MonoBehaviour @object)
        {
            Owner = @object;
            ElapsedTime = Time.time;
            yield return OnAction();
        }
    }
}