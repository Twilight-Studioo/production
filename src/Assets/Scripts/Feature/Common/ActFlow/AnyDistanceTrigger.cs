#region

using System.Collections.Generic;
using System.Linq;
using DynamicActFlow.Runtime.Core;
using DynamicActFlow.Runtime.Core.Action;
using UnityEngine;

#endregion

namespace Feature.Common.ActFlow
{
    [TriggerTag("AnyDistance")]
    public class AnyDistanceTrigger : TriggerBase
    {
        [TriggerParameter("Distances")] private List<float> Distances { get; set; }

        [TriggerParameter("TargetTransform")] private Transform TargetPosition { get; set; }

        public override void OnCreated()
        {
            Distances = new();
            TargetPosition = null;
        }

        public override bool IfEnd(MonoBehaviour owner)
        {
            if (Distances.Count == 0 || TargetPosition == null)
            {
                return true;
            }

            return Distances.Any(distance =>
                Vector3.Distance(owner.transform.position, TargetPosition.position) < distance);
        }
    }
}