#region

using System;
using DynamicActFlow.Runtime.Core;
using DynamicActFlow.Runtime.Core.Action;
using UnityEngine;
using UnityEngine.AI;

#endregion

namespace Feature.Common.ActFlow
{
    [ActionTag("AIMoveToTargetDistance")]
    public class AIMoveToTargetDistance : FixedUpdatedAction
    {
        private NavMeshAgent agent;

        private Vector3 targetPoint;
        [ActionParameter("Target")] private Transform Target { get; set; }
        [ActionParameter("Distance")] private float TargetDistance { get; set; }

        public override void OnCreated()
        {
            base.OnCreated();
            Target = null;
            TargetDistance = 1.0f;
        }

        protected override void Start()
        {
            agent = Owner.GetComponent<NavMeshAgent>();
            targetPoint = Target.position;
        }

        protected override void FixedUpdate()
        {
            if (Target == null)
            {
                return;
            }

            // プレイヤーとの距離を計算
            var distanceToPlayer = Vector3.Distance(targetPoint, Owner.transform.position);

            if (distanceToPlayer < TargetDistance)
            {
                // プレイヤーから離れる位置を計算
                var directionAwayFromPlayer = (Owner.transform.position - targetPoint).normalized;
                var targetPosition = targetPoint + directionAwayFromPlayer * TargetDistance;

                // NavMeshAgentに目標位置を設定
                agent.SetDestination(targetPosition);
            }
            else if (distanceToPlayer > TargetDistance)
            {
                // プレイヤーに近づく位置を計算
                var directionToPlayer = (targetPoint - Owner.transform.position).normalized;
                var targetPosition = targetPoint - directionToPlayer * TargetDistance;

                // NavMeshAgentに目標位置を設定
                agent.SetDestination(targetPosition);
            }
        }

        protected override bool CheckIfEnd() =>
            Math.Abs(Vector3.Distance(Owner.transform.position, targetPoint) - TargetDistance) < 0.1f || Target == null;
    }
}