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
    public class SimpleEnemy1Agent : FlowScope, IEnemyAgent, ISwappable
    {
        public List<Vector3> points;

        private NavMeshAgent agent;

        private SimpleEnemy1Params enemyParams;

        private OnHitRushAttack onHitRushAttack;

        private Transform playerTransform;
        
        private readonly IReactiveProperty<Vector2> position = new ReactiveProperty<Vector2>();

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            position.Value = transform.position;
        }

        private void Update()
        {
            position.Value = transform.position;
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
            playerTransform = player;
        }

        public void OnDamage(uint damage, Vector3 hitPoint, Transform attacker)
        {
            var imp = (transform.position - attacker.position).normalized;
            imp.y += 10f;
            StartCoroutine(transform.Knockback(imp, 10f, 0.5f));
        }

        public event Action OnTakeDamageEvent;
        public event Action<ISwappable> OnAddSwappableItem;

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
                    agent.ResetPath();
                    yield return Action("AIMoveToFollow")
                        .Param("FollowTransform", playerTransform)
                        .Param("MoveSpeed", enemyParams.pursuitSpeed)
                        .IfEnd(
                            UnFocusTrigger()
                                .Build(),
                            RushStart()
                                .Build()
                        )
                        .Build();
                }
            }
        }

        private IEnumerator Attack()
        {
            onHitRushAttack = TakeDamage;
            agent.ResetPath();
            yield return Wait(enemyParams.rushBeforeDelay);
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
            OnTakeDamageEvent?.Invoke();
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
            transform.position = p;
        }

        public event Action OnDestroyEvent;
    }
}