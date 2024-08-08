#region

using DynamicActFlow.Runtime.Core;
using DynamicActFlow.Runtime.Core.Action;
using UnityEngine;

#endregion

namespace Feature.Common.ActFlow
{
    [TriggerTag("PlayerDistance")]
    public class PlayerDistanceTrigger : TriggerBase
    {
        private Transform playerTransform;
        [TriggerParameter("Distance")] private float Distance { get; set; }

        public override void OnCreated()
        {
            Distance = 1f;
        }

        public override void Start()
        {
            base.Start();
            playerTransform = GameObject.FindWithTag("Player").transform;
        }

        public override bool IfEnd(MonoBehaviour owner) =>
            Vector3.Distance(owner.transform.position, playerTransform.position) < Distance;
    }
}