#region

using System;
using System.Collections;
using System.Collections.Generic;
using Core.Utilities;
using DynamicActFlow.Runtime.Core.Action;
using DynamicActFlow.Runtime.Core.Flow;
using Feature.Common.ActFlow;
using Feature.Common.Parameter;
using Feature.Interface;
using UnityEngine;
using UnityEngine.AI;

#endregion

namespace Feature.Enemy
{
    public class SimpleEnemy1Agent : FlowScope, IEnemyAgent
    {
        public List<Vector3> points;

        private OnHitRushAttack onHitRushAttack;

        private NavMeshAgent agent;

        private SimpleEnemy1Params enemyParams;

        private Transform playerTransform;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }


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

        public void SetPatrolPoints(List<Vector3> pts)
        {
            points = pts;
        }

        public void SetPlayerTransform(Transform player)
        {
            Debug.Log("SetPlayerTransform", player);
            playerTransform = player;
        }

        private TriggerRef MoveTrigger() =>
            Trigger("AnyDistance")
                .Param("Distances", new List<float> { enemyParams.rushStartDistance, enemyParams.foundDistance, })
                .Param("TargetTransform", playerTransform);

        private TriggerRef RushStart() =>
            Trigger("Distance")
                .Param("Target", enemyParams.rushStartDistance)
                .Param("Object", playerTransform);

        private TriggerRef UnFocusTrigger() =>
            Trigger("Distance")
                .Param("Target", enemyParams.pursuitDistance)
                .Param("IsClose", false)
                .Param("Object", playerTransform);

        protected override IEnumerator Flow(IFlowBuilder context)
        {
            if (enemyParams == null)
            {
                throw new("EnemyParams is not set");
            }

            while (true)
            {
                Debug.Log($"Patrol {Time.time}");
                agent.ResetPath();
                yield return Action("PointsAIMoveTo")
                    .Param("Points", points)
                    .Param("MoveSpeed", enemyParams.patrolSpeed)
                    .IfEnd(
                        MoveTrigger()
                            .Build()
                    )
                    .Build();
                var distance = Vector3.Distance(playerTransform.position, transform.position);
                if (enemyParams.rushStartDistance > distance)
                {
                    yield return Attack();
                }

                if (enemyParams.foundDistance > distance)
                {
                    Debug.Log("Pursuit");
                    agent.ResetPath();
                    yield return Action("AIMoveToFollow")
                        .Param("FollowTransform", playerTransform)
                        .Param("MoveSpeed", enemyParams.pursuitSpeed)
                        .IfEnd(new[]
                        {
                            UnFocusTrigger()
                                .Build(),
                            RushStart()
                                .Build(),
                        })
                        .Build();
                }
            }
        }

        private IEnumerator Attack()
        {
            onHitRushAttack = TakeDamage;
            Debug.Log("Rush Wait");
            agent.ResetPath();
            yield return Wait(enemyParams.rushBeforeDelay);
            Debug.Log("Rush");
            yield return Action("AIRushToPosition")
                .Param("RushSpeed", enemyParams.rushSpeed)
                .Param("TargetTransform", playerTransform)
                .Param("OnHitRushAttack", onHitRushAttack)
                .Build();
            yield return Wait(enemyParams.rushAfterDelay);
        }
        
        private void TakeDamage()
        {
            var player = ObjectFactory.FindPlayer();
            if (player == null)
            {
                return;
            }

            var view = player.GetComponent<IDamaged>();
            view.OnDamage(enemyParams.damage, transform.position, transform);
        }

        public void OnDamage(uint damage, Vector3 hitPoint, Transform attacker)
        {
            var imp = (transform.position - attacker.position).normalized;
            imp.y += 10f;
            StartCoroutine(transform.Knockback(imp, 10f, 0.5f));
        }

        public event Action OnTakeDamageEvent;
    }
}