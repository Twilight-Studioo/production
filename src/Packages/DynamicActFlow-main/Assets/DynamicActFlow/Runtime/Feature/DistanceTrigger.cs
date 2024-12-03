#region

using System;
using DynamicActFlow.Runtime.Core;
using DynamicActFlow.Runtime.Core.Action;
using UnityEngine;

#endregion

namespace DynamicActFlow.Runtime.Feature
{
    [TriggerTag("Distance")]
    public sealed class DistanceTrigger : TriggerBase
    {
        [TriggerParameter("Object")] private Transform TargetTransform { get; set; }
        [TriggerParameter("Target", 1f)] private float TargetDistance { get; set; }
        [TriggerParameter("IsClose", true)] private bool IsClose { get; set; }

        public override void OnCreated()
        {
            TargetDistance = 1f;
            TargetTransform = null;
            IsClose = true;
        }

        public override void Start()
        {
            base.Start();
            if (!TargetTransform)
            {
                throw new NotImplementedException("TargetTransform is null");
            }
        }

        public override bool IfEnd(MonoBehaviour owner)
        {
            if (IsClose)
            {
                return Vector3.Distance(owner.transform.position, TargetTransform.position) < TargetDistance;
            }

            return Vector3.Distance(owner.transform.position, TargetTransform.position) > TargetDistance;
        }
    }
}