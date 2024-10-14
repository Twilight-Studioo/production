#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Utilities;
using DynamicActFlow.Runtime.Core.Action;
using DynamicActFlow.Runtime.Core.Flow;
using Feature.Common.ActFlow;
using Feature.Common.Constants;
using Feature.Common.Parameter;
using Feature.Interface;
using UnityEngine;
using UnityEngine.VFX;

#endregion

namespace Feature.Component.Enemy
{
    public class DroneAgent : FlowScope, IEnemyAgent
    {
        [SerializeField] private GameObject beamPrefab;
        private VisualEffect beamEffect;

        private DroneParams enemyParams;
        private Transform playerTransform;

        private Rigidbody rb;
        private SelfCheck thresholdCheck;

        private DroneAttackType AttackType => enemyParams.attackType;

        private void OnDrawGizmos()
        {
            // 自爆範囲
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, enemyParams.explosionRadius);
        }

        public EnemyType EnemyType => EnemyType.Drone;

        public Action RequireDestroy { set; get; }

        public void FlowExecute()
        {
            thresholdCheck = CheckThresholdHealth;
            rb = GetComponent<Rigidbody>();
            playerTransform = ObjectFactory.Instance.FindPlayer()?.transform;
            FlowStart();
        }

        public void FlowCancel()
        {
            FlowStop();
        }

        public void SetParams(EnemyParams @params)
        {
            if (@params is DroneParams droneParams)
            {
                enemyParams = droneParams;
            }
        }

        public void SetPatrolPoints(List<Vector3> pts)
        {
        }

        public void OnDamage(uint damage, Vector3 hitPoint, Transform attacker)
        {
        }


        public GetHealth OnGetHealth { set; get; }

#pragma warning disable CS0067
        public event Action OnTakeDamageEvent;
#pragma warning restore CS0067

        protected override IEnumerator Flow(IFlowBuilder context)
        {
            yield return InfinityWait(1f)
                .Build();
            while (true)
            {
                var distance = Vector3.Distance(playerTransform.position, transform.position);

                if (distance > enemyParams.playerKeepDistance)
                {
                    yield return Action("FlyingEnemy")
                        .Param("Transform", playerTransform)
                        .Param("PlayerMinDistance", enemyParams.playerKeepDistance)
                        .IfEnd(
                            Trigger("PlayerDistance")
                                .Param("Distance", enemyParams.playerKeepDistance)
                                .Build(),
                            Trigger("SelfCheck")
                                .Param("Trigger", thresholdCheck)
                                .Build()
                        )
                        .Build();
                    rb.velocity = Vector3.zero;
                }
                else if (CheckThresholdHealth())
                {
                    var startTimer = Time.time;
                    // TODO
                    // 自爆
                    yield return Action("FlyMoveToFollow")
                        .Param("FollowTransform", playerTransform)
                        .Param("FinishDistance", 2f)
                        .Param("MoveSpeed", enemyParams.selfDestructionMoveSpeed)
                        .IfEnd(
                            Trigger("WaitTrigger")
                                .Param("Seconds", enemyParams.selfDestructionCountDownSec)
                                .Build()
                        )
                        .Build();

                    while (Time.time - startTimer < enemyParams.selfDestructionCountDownSec)
                    {
                        yield return new WaitForFixedUpdate();
                    }

                    transform.GetCircleCastAll(enemyParams.explosionRadius, transform.forward, 0)
                        .ToList()
                        .ForEach(hit =>
                        {
                            if (hit.distance < enemyParams.explosionRadius)
                            {
                                if (hit.collider is null || !hit.collider.CompareTag("Player"))
                                {
                                    return;
                                }

                                hit.collider.GetComponent<IDamaged>()?
                                    .OnDamage(enemyParams.explosionDamage, transform.position, transform);
                            }
                        });
                    RequireDestroy?.Invoke();
                    yield break;
                }
                else
                {
                    yield return Attack();
                }
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private bool CheckThresholdHealth()
        {
            var health = OnGetHealth?.Invoke();
            return health.HasValue && health.Value <= enemyParams.thresholdHealth;
        }

        private IEnumerator Attack()
        {
            switch (AttackType)
            {
                case DroneAttackType.Bullet:
                    for (var _ = 0; _ < enemyParams.shotCount; _++)
                    {
                        var bullet = ObjectFactory.Instance.CreateObject(enemyParams.bulletPrefab, transform.position,
                            Quaternion.identity);
                        var bulletRb = bullet.GetComponent<DamagedTrigger>();
                        bulletRb.SetHitObject(false, true, true);
                        bulletRb.ExecuteWithFollow(playerTransform, enemyParams.bulletSpeed, enemyParams.damage,
                            enemyParams.bulletLifeTime);
                        yield return Wait(enemyParams.shootIntervalSec);
                    }

                    break;
                case DroneAttackType.Ray:
                    var dir = (playerTransform.position - transform.position).normalized;
                    if (beamEffect is null)
                    {
                        var effect = ObjectFactory.Instance.CreateObject(beamPrefab, transform.position,
                            Quaternion.LookRotation(dir), transform);
                        if (effect.TryGetComponentInChild<VisualEffect>(out var component))
                        {
                            beamEffect = component;
                        }
                    }
                    
                    if (beamEffect is null)
                    {
                        Debug.LogError("BeamEffect is null");
                        yield break;
                    }

                    yield return new WaitForSeconds(enemyParams.rayWaitSec);

                    beamEffect.transform.LookAt(playerTransform);
                    beamEffect.SetFloat("Duration", enemyParams.rayIntervalSec);
                    beamEffect.SetFloat("Length", enemyParams.rayRange);
                    beamEffect.Play();
                    transform.GetBoxCastAll(Vector3.one, dir, enemyParams.rayRange)
                        .ToList()
                        .ForEach(hit =>
                        {
                            if (hit.collider is not null && hit.collider.CompareTag("Player"))
                            {
                                hit.collider.GetComponent<IDamaged>()?
                                    .OnDamage(enemyParams.rayDamage, transform.position, transform);
                            }
                        });
                    yield return new WaitForSeconds(enemyParams.rayIntervalSec);
                    beamEffect.Stop();

                    break;
                case DroneAttackType.None:
                    break;
            }
        }
    }
}