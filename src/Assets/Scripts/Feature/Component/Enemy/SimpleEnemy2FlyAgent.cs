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
    public class SimpleEnemy2FlyAgent : FlowScope, IEnemyAgent, ISwappable
    {
        [SerializeField] private GameObject bulletPrefab;

        private readonly IReactiveProperty<Vector2> position = new ReactiveProperty<Vector2>();

        private Rigidbody rb;

        private bool canBullet = false;

        private SimpleEnemy2FlyParams enemyParams;

        private OnHitRushAttack onHitBullet;

        private Transform playerTransform;
        private List<Vector3> points;
        
        private float lastAttackedTime;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            position.Value = transform.position;
        }

        private void Update()
        {
            position.Value = transform.position;
        }

        public Action RequireDestroy { set; get; }

        public GetHealth OnGetHealth { get; set; }
        public EnemyType EnemyType => EnemyType.SimpleEnemy2;

        private Coroutine movableCoroutine;


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
            if (@params is SimpleEnemy2FlyParams enemy2FlyParams)
            {
                enemyParams = enemy2FlyParams;
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
            yield return transform.Knockback(imp, 3f, 0.8f);
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

        private void FixedUpdate()
        {
            if (!canBullet && Time.time - lastAttackedTime > enemyParams.shootCoolDownSec)
            {
                canBullet = true;
            }
        }

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
            canBullet = true;
            if (movableCoroutine != null)
            {
                StopCoroutine(movableCoroutine);   
            }
            movableCoroutine = StartCoroutine(Movable());

            while (true)
            {
                // パトロール状態

                var distance = Vector3.Distance(playerTransform.position, transform.position);
                
                if (enemyParams.pursuitDistance < distance)
                {
                    yield return new WaitForSeconds(0.5f);
                    continue;
                }

                if (Math.Abs(enemyParams.shootDistance - distance) < 2f && distance > 1f && canBullet)
                {
                    Debug.Log("Attack");
                    canBullet = false;
                    lastAttackedTime = Time.time;
                    yield return Attack();
                }

                yield return new WaitForSeconds(0.2f);
            }
        }

        private IEnumerator Movable()
        {
            if (playerTransform == null)
            {
                playerTransform = ObjectFactory.Instance.FindPlayer()?.transform;
            }
            yield return Action("FlyingEnemyV2")
                .Param("Target", playerTransform)
                .Param("Power", enemyParams.movePower)
                .Param("Distance", enemyParams.distance)
                .Param("MaxHeightFromGround", enemyParams.maxHeightFromGround)
                .Param("MinDistanceToCeiling", enemyParams.minDistanceToCeiling)
                .Build();
            rb.velocity = Vector3.zero;
        }

        private IEnumerator Attack()
        {
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

        public void Delete()
        {
            OnDestroyEvent?.Invoke();
            Destroy(gameObject);
        }

        public void DestroyEnemy()
        {
            
        }
    }
}