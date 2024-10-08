#region

using System;
using System.Collections;
using System.Collections.Generic;
using DynamicActFlow.Runtime.Core.Action;
using DynamicActFlow.Runtime.Core.Flow;
using Feature.Common.ActFlow;
using Feature.Common.Constants;
using Feature.Common.Parameter;
using Feature.Interface;
using UnityEngine;

#endregion

namespace Feature.Component.Enemy
{
    public class DroneAgent : FlowScope, IEnemyAgent
    {
        private DroneParams enemyParams;
        private Transform playerTransform;

        private Rigidbody rb;
        private SelfCheck thresholdCheck;

        private DroneAttackType AttackType => enemyParams.attackType;
        public EnemyType EnemyType => EnemyType.Drone;

        public void FlowExecute()
        {
            thresholdCheck = CheckThresholdHealth;
            rb = GetComponent<Rigidbody>();
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

        public void SetPlayerTransform(Transform playerTransform)
        {
            this.playerTransform = playerTransform;
        }

        public void SetPatrolPoints(List<Vector3> pts)
        {
        }

        public void OnDamage(uint damage, Vector3 hitPoint, Transform attacker)
        {
        }


        public GetHealth OnGetHealth { set; get; }

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
                    // TODO
                    // 自爆
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
                        var bullet = Instantiate(enemyParams.bulletPrefab, transform.position, Quaternion.identity);
                        var bulletRb = bullet.GetComponent<DamagedTrigger>();
                        bulletRb.SetHitObject(false, true, true);
                        bulletRb.ExecuteWithFollow(playerTransform, enemyParams.bulletSpeed, enemyParams.damage, enemyParams.bulletLifeTime);
                        OnAddSwappableItem?.Invoke(bulletRb);
                        yield return Wait(enemyParams.shootIntervalSec);
                    }

                    break;
                case DroneAttackType.Ray:
                    // TODO ビームの仕様で実装
                    Debug.Log("Ray");
                    break;
                case DroneAttackType.None:
                    break;
            }
        }

#pragma warning disable CS0067
        public event Action OnTakeDamageEvent;
        public event Action<ISwappable> OnAddSwappableItem;
#pragma warning restore CS0067
    }
}