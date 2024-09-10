using System;
using System.Collections;
using System.Collections.Generic;
using Core.Utilities;
using DynamicActFlow.Runtime.Core.Action;
using DynamicActFlow.Runtime.Core.Flow;
using Feature.Common.ActFlow;
using Feature.Common.Parameter;
using Feature.Enemy;
using UnityEngine;
using UnityEngine.AI;

namespace Feature.Component.Enemy
{
    public class SimpleEnemy2Agent : FlowScope, IEnemyAgent
    {
        private List<Vector3> points;

        private NavMeshAgent agent;

        private SimpleEnemy2Params enemyParams;

        private OnHitRushAttack onHitBullet;

        private Transform playerTransform;
        
        [SerializeField]
        private GameObject bulletPrefab;

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
            if (@params is SimpleEnemy2Params enemy1Params)
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
            playerTransform = player;
        }

        public void OnDamage(uint damage, Vector3 hitPoint, Transform attacker)
        {
            var imp = (transform.position - attacker.position).normalized;
            imp.y += 10f;
            StartCoroutine(transform.Knockback(imp, 10f, 0.5f));
        }

        public event Action OnTakeDamageEvent;

        private TriggerRef FocusTrigger() =>
            Trigger("Distance")
                .Param("Target", enemyParams.foundDistance)
                .Param("IsClose", true)
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
                agent.ResetPath();
                Debug.Log("Patrol");
                yield return Action("PointsAIMoveTo")
                    .Param("Points", points)
                    .Param("MoveSpeed", enemyParams.patrolSpeed)
                    .IfEnd(
                        new []
                        {
                            FocusTrigger()
                                .Build(),
                            UnFocusTrigger()
                                .Build(),
                        }
                    )
                    .Build();
                var distance = Vector3.Distance(playerTransform.position, transform.position);
                if (Math.Abs(enemyParams.shootDistance - distance) < 0.2f)
                {
                    yield return Attack();
                }

                if (enemyParams.foundDistance > distance)
                {
                    agent.ResetPath();
                    Debug.Log("MoveToBeforeShoot");
                    yield return Action("AIMoveToTargetDistance")
                        .Param("Target", playerTransform)
                        .Param("Distance", enemyParams.shootDistance)
                        .Param("MoveSpeed", 1f)
                        .IfEnd(
                            UnFocusTrigger()
                                .Build()
                        )
                        .Build();
                }
            }
        }

        private IEnumerator Attack()
        {
            var dir = (playerTransform.position - transform.position).normalized;
            for (var _ = 0; _ < enemyParams.shootCount; _++)
            {
                Debug.Log("Bullet");
                var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                var bulletRb = bullet.GetComponent<DamagedTrigger>();
                bulletRb.SetHitObject(false, true);
                bulletRb.Execute(dir, enemyParams.shootSpeed, enemyParams.damage);
                yield return Wait(enemyParams.shootIntervalSec);
            }

            yield return Wait(enemyParams.shootAfterSec);

        }
    }
}