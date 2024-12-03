#region

using System.Collections;
using UnityEngine;

#endregion

namespace DynamicActFlow.Runtime.Core.Action
{
    public class ActionRef : Ref
    {
        private readonly ActionBase action;
        private readonly MonoBehaviour owner;

        internal ActionRef(ActionBase action, MonoBehaviour owner)
        {
            this.action = action;
            this.owner = owner;
        }

        internal override void SetParam(string key, object param)
        {
            action.SetProperty(key, param);
        }

        internal void SetTrigger(TriggerBase trigger)
        {
            action.SetTrigger(trigger);
        }

        internal void SetTriggers(TriggerBase[] triggers)
        {
            action.SetTriggers(triggers);
        }

        internal void SetTimeout(float timeout)
        {
            action.SetTimeout(timeout);
        }

        public IEnumerator Build()
        {
            yield return owner.StartCoroutine(action.ExecuteActionCoroutine(owner));
        }

        public ActionType Type()
        {
            return action switch
            {
                FixedUpdatedAction => ActionType.FixedUpdated,
                UpdatedAction => ActionType.Updated,
                CalledAction => ActionType.Called,
                _ => throw new("Unknown action type"),
            };
        }
    }
}