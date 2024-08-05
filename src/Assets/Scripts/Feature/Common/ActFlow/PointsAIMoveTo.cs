#region

using System.Collections.Generic;
using DynamicActFlow.Runtime.Core;
using DynamicActFlow.Runtime.Core.Action;
using Feature.Common.Constants;
using UnityEngine;
using UnityEngine.AI;

#endregion

namespace Feature.Common.ActFlow
{
    [ActionTag("PointsAIMoveTo")]
    public sealed class PointsAIMoveTo : FixedUpdatedAction
    {
        private NavMeshAgent agent;

        private int currentIdx;
        [ActionParameter("Points")] private List<Vector3> TargetPoints { get; set; }

        [ActionParameter("MoveSpeed")] private float MoveSpeed { get; set; }

        public override void OnCreated()
        {
            base.OnCreated();
            TargetPoints = new();
            MoveSpeed = 1.0f;
        }

        protected override void Start()
        {
            agent = Owner.GetComponent<NavMeshAgent>();
            currentIdx = 0;
            agent.avoidancePriority = 50;
            agent.autoRepath = true; // 自動経路再計算を有効にする
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            agent.SetDestination(TargetPoints[currentIdx]);
            agent.speed = ParameterEx.MergePlayerAgentSpeed(3.5f * MoveSpeed);
        }

        protected override void FixedUpdate()
        {
            if (agent.remainingDistance < 0.1f)
            {
                currentIdx++;
                if (currentIdx >= TargetPoints.Count)
                {
                    currentIdx = 0;
                }
                
            }
            agent.SetDestination(TargetPoints[currentIdx]);
        }

        protected override bool CheckIfEnd() => TargetPoints.Count == 0;
    }
}