#region

using System.Collections;
using DynamicActFlow.Runtime.Core;
using DynamicActFlow.Runtime.Core.Action;
using UnityEngine;

#endregion

namespace DynamicActFlow.Runtime.Feature
{
    [ActionTag(ActionName.Wait)]
    public sealed class WaitAction : CalledAction
    {
        [ActionParameter("Seconds", 1f)] private float Seconds { get; set; }

        public override void OnCreated()
        {
            base.OnCreated();
            Seconds = 1f;
        }

        protected override IEnumerator Call()
        {
            yield return new WaitForSeconds(Seconds);
        }
    }
}