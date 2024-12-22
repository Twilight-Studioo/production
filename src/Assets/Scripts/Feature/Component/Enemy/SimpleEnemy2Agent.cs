#region

using System;
using System.Collections;
using System.Collections.Generic;
using Core.Utilities;
using DynamicActFlow.Runtime.Core.Action;
using DynamicActFlow.Runtime.Core.Flow;
using Feature.Common.ActFlow;
using Feature.Common.Constants;
using Feature.Common.Parameter;
using Feature.Interface;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

#endregion

namespace Feature.Component.Enemy
{
    public class SimpleEnemy2Agent : FlowScope, IEnemyAgent, ISwappable
    {
        [SerializeField] private GameObject bulletPrefab;

        private readonly IReactiveProperty<Vector2> position = new ReactiveProperty<Vector2>();

        private NavMeshAgent agent;
        private Rigidbody rb;

        private bool canBullet;

        private SimpleEnemy2Params enemyParams;

        private OnHitRushAttack onHitBullet;

        private Transform playerTransform;
        private List<Vector3> points;

        private Animator animator;
        private bool loseAnimation = false;
        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            rb = GetComponent<Rigidbody>();
            position.Value = transform.position;
            animator = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            position.Value = transform.position;
        }
        public Action RequireDestroy { set; get; }

        public GetHealth OnGetHealth { get; set; }
        public EnemyType EnemyType => EnemyType.SimpleEnemy2;


        public void FlowCancel()
        {
            FlowStop();
        }

        public void FlowExecute()
        {
            playerTransform = ObjectFactory.Instance.FindPlayer()?.transform;
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

        public void OnDamage(uint damage, Vector3 hitPoint, Transform attacker)
        {
            var imp = (transform.position - attacker.position).normalized;
            imp.y += 0.3f;
            this.UniqueStartCoroutine(HitStop(imp), $"HitStop_{gameObject.name}");;
        }
        
        private IEnumerator HitStop(Vector3 imp)
        {
            FlowCancel();
            agent.enabled = false;
            rb.isKinematic = false;
            yield return transform.Knockback(imp, 3f, 0.8f);
            rb.isKinematic = true;
            agent.enabled = true;
            FlowStart();
        }
#pragma warning disable CS0067
        public event Action OnTakeDamageEvent;
#pragma warning disable CS0067

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

        private TriggerRef AttackTrigger() =>
            Trigger("Distance")
                .Param("Target", enemyParams.shootDistance)
                .Param("IsClose", true)
                .Param("Object", playerTransform);

        protected override IEnumerator Flow(IFlowBuilder context)
        {
            if (enemyParams == null)
            {
                throw new("EnemyParams is not set");
            }

            while (playerTransform == null)
            {
                playerTransform = ObjectFactory.Instance.FindPlayer()?.transform;
                yield return Wait(0.5f);
            }

            while (true)
            {
                if (!loseAnimation)
                {
                    // パトロール状態
                    agent.ResetPath();

                    var distance = Vector3.Distance(playerTransform.position, transform.position);

                    if (Math.Abs(enemyParams.shootDistance - distance) < 1f || canBullet)
                    {
                        canBullet = false;
                        yield return Attack();
                    }
                    else if (enemyParams.pursuitDistance > distance)
                    {
                        yield return Action("AIMoveToTargetDistance")
                            .Param("Target", playerTransform)
                            .Param("Distance", enemyParams.shootDistance)
                            .Param("MoveSpeed", 1f)
                            .IfEnd(
                                UnFocusTrigger().Build(), // プレイヤーを見失った場合のトリガー
                                AttackTrigger().Build()
                            )
                            .Build();
                        canBullet = true;
                    }
                    else if((enemyParams.foundDistance > distance))
                    {
                        yield return Action("PointsAIMoveTo")
                            .Param("Points", points)
                            .Param("MoveSpeed", enemyParams.patrolSpeed)
                            .IfEnd(
                                FocusTrigger().Build(), // プレイヤー発見のトリガー
                                AttackTrigger().Build() // 攻撃モードのトリガー
                            )
                            .Build();
                    }
                    else
                    {
                        animator.Play("miss");
                        yield return Wait(5.0f);
                    }
                }

            }
        }

        private IEnumerator Attack()
        {
            if (!loseAnimation)
            {
                animator.Play("attackSetB");
                yield return Wait(1.5f);
                animator.Play("attackB");
                var dir = (playerTransform.position - transform.position).normalized;
                for (var _ = 0; _ < enemyParams.shootCount; _++)
                {
                    var bullet = ObjectFactory.Instance.CreateObject(
                        bulletPrefab,
                        transform.position + dir * 1f,
                        Quaternion.identity);
                    bullet.transform.LookAt(playerTransform);
                    var bulletRb = bullet.GetComponent<DamagedTrigger>();
                    bulletRb.SetHitObject(true, true, true);
                    bulletRb.Execute(dir, enemyParams.shootSpeed, enemyParams.damage, enemyParams.bulletLifeTime);
                    bulletRb.OnHitEvent += () => onHitBullet?.Invoke();
                    yield return Wait(enemyParams.shootIntervalSec);
                }
                
                yield return Wait(enemyParams.shootAfterSec);
            }

        }

        public void OnSelected()
        {
        }

        public void OnDeselected()
        {
        }

        public void OnInSelectRange()
        {
        }

        public void OnOutSelectRange()
        {
        }

        public IReadOnlyReactiveProperty<Vector2> GetPositionRef() => position;

        public Vector2 GetPosition() => transform.position;

        public void OnSwap(Vector2 p)
        {
            agent.Warp(p);
        }

        public event Action OnDestroyEvent;

        public void Delete()
        {
            OnDestroyEvent?.Invoke();
            Destroy(gameObject);
        }
        public void DestroyEnemy()
        {
            loseAnimation = true;
            animator.Play("defeat");
        }
    }
}