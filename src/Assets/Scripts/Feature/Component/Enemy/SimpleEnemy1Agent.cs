#region

using System;
using System.Collections;
using System.Collections.Generic;
using Codice.Client.BaseCommands;
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
using UnityEngine.Rendering.UI;

#endregion

namespace Feature.Component.Enemy
{
    public class SimpleEnemy1Agent : FlowScope, IEnemyAgent, ISwappable
    {
        public List<Vector3> points;

        private readonly IReactiveProperty<Vector2> position = new ReactiveProperty<Vector2>();

        private NavMeshAgent agent;
        private Rigidbody rb;

        private SimpleEnemy1Params enemyParams;

        private OnHitRushAttack onHitRushAttack;

        private Transform playerTransform;

        private Animator animator;

        private bool loseAnimation = false;
        private bool tracking = false;

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
        public EnemyType EnemyType => EnemyType.SimpleEnemy1;
        
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

        public event Action OnTakeDamageEvent;

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

#pragma warning disable CS0067
        public event Action OnDestroyEvent;
#pragma warning restore CS0067

        public void Delete()
        {
            OnDestroyEvent?.Invoke();
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
            if (loseAnimation)
            {
                yield break;
            }
            if (enemyParams == null)
            {
                throw new("EnemyParams is not set");
            }

            while (playerTransform == null)
            {
                yield return Wait(0.5f);
            }

            while (true)
            {
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
                    if (!tracking)
                    {
                        animator.Play("float");  
                    }
                    tracking = true;
                    agent.ResetPath();
                    yield return Action("AIMoveToFollow")
                        .Param("FollowTransform", playerTransform)
                        .Param("MoveSpeed", enemyParams.pursuitSpeed)
                        .IfEnd(
                            UnFocusTrigger()
                                .Build()
                            // RushStart()
                            //     .Build()
                        )
                        .Build();
                }
                else
                {
                    if (tracking)
                    {
                        animator.Play("move");  
                    }
                    tracking = false;
                    animator.Play("miss");
                    yield return Wait(5.0f);
                }
            }
        }
        
        private IEnumerator Attack()
        { 
            onHitRushAttack = TakeDamage;
            agent.ResetPath();
            yield return Wait(enemyParams.rushBeforeDelay);
            animator.Play("attackSetA");
            yield return Wait(0.2f);
            if (loseAnimation)
            {
               yield break;
            }
            AttackDecision();
             yield return Action("AIRushToPosition")
                 .Param("RushSpeed", enemyParams.rushSpeed)
                 .Param("TargetTransform", playerTransform)
            //     .Param("OnHitRushAttack", onHitRushAttack)
                 .Build();
            yield return Wait(enemyParams.rushAfterDelay);
        }

        public void TakeDamage()
        {
            var player = ObjectFactory.Instance.FindPlayer();
            if (player == null)
            {
                return;
            }

            if (!loseAnimation)
            {
                var view = player.GetComponent<IDamaged>();
                view.OnDamage(enemyParams.damage, transform.position, transform);
                OnTakeDamageEvent?.Invoke();
            }

        }

        private void AttackDecision()
        {
            if (!loseAnimation)
            {
                animator.Play("attackA");
            }
        }
        
        public void DestroyEnemy()
        {
           // loseAnimation = true;
            FlowCancel();
            animator.Play("defeat");
        }
    }
}