#region

using System.Collections;
using System.Collections.Generic;
using DynamicActFlow.Runtime.Core.Action;
using DynamicActFlow.Runtime.Core.Flow;
using Feature.Common.ActFlow;
using Feature.Common.Parameter;
using UnityEngine;

#endregion

namespace Feature.Enemy
{
    public class SimpleEnemy1Agent : FlowScope, IEnemyAgent
    {
        [SerializeField] public List<Vector3> points;

        private readonly OnHitRushAttack onHitRushAttack = () => { Debug.Log("Rush Attack"); };

        private SimpleEnemy1Params enemyParams;

        private Transform playerTransform;


        public void FlowCancel()
        {
            FlowStop();
        }

        public void FlowExecute()
        {
            FlowStart();
        }

        public void SetParams(EnemyParams @params)
        {
            if (@params is SimpleEnemy1Params enemy1Params)
            {
                enemyParams = enemy1Params;
            }
            else
            {
                throw new($"Invalid EnemyParams {enemyParams}");
            }
        }

        public void SetPlayerTransform(Transform player)
        {
            playerTransform = player;
        }

        private TriggerRef MoveTrigger() =>
            Trigger("AnyDistance")
                .Param("Distances", new List<float> { enemyParams.rushStartDistance, enemyParams.pursuitDistance, })
                .Param("TargetTransform", playerTransform);

        protected override IEnumerator Flow(IFlowBuilder context)
        {
            if (enemyParams == null)
            {
                throw new("EnemyParams is not set");
            }

            while (true)
            {
                Debug.Log("Patrol");
                yield return Action("PointsAIMoveTo")
                    .Param("Points", points)
                    .Param("MoveSpeed", enemyParams.patrolSpeed)
                    .IfEnd(
                        MoveTrigger()
                            .Build()
                    )
                    .Build();
                if (enemyParams.rushStartDistance > Vector3.Distance(playerTransform.position, transform.position))
                {
                    yield return Attack();
                }
                else
                {
                    Debug.Log("Pursuit");
                    yield return Action("AIMoveToFollow")
                        .Param("FollowTransform", playerTransform)
                        .Param("MoveSpeed", enemyParams.pursuitSpeed)
                        .IfEnd(
                            MoveTrigger()
                                .Build()
                        )
                        .Build();
                }
            }
        }

        private IEnumerator Attack()
        {
            Debug.Log("Rush Wait");
            yield return Wait(enemyParams.rushBeforeDelay);
            Debug.Log("Rush");
            yield return Action("AIRushToPosition")
                .Param("RushSpeed", enemyParams.rushSpeed)
                .Param("TargetTransform", playerTransform)
                .Param("OnHitRushAttack", onHitRushAttack)
                .Build();
        }
    }
}