using System;
using System.Collections;
using Core.Utilities;
using Core.Utilities.Health;
using DynamicActFlow.Runtime.Core.Flow;
using Feature.Component.Enemy.SmasherEx.State;
using Feature.Interface;
using UnityEngine;
using UnityEngine.AI;

namespace Feature.Component.Enemy.SmasherEx
{
    public partial class SmasherAI: FlowScope, IHealthBar, IDisposable, IDamaged
    {
        [SerializeField] private GameObject minePrefab;
        [SerializeField] private DebrisSpawner debrisSpawner;
        private Rigidbody rb;
        private NavMeshAgent agent;

        /// <summary>
        /// 地雷をplayer後方に向かって山形に投げる
        /// </summary>
        private partial IEnumerator ThrowingMines();
        
        /// <summary>
        /// playerの方向(前後)に突進する
        /// playerにぶつかると仰け反らせ、地雷に当てようとする
        /// </summary>
        private partial IEnumerator ChargeForward();

        /// <summary>
        /// 前方に向かって攻撃
        /// only ground attack
        /// </summary>
        private partial IEnumerator ForwardBlow();
        
        /// <summary>
        /// 何かしらのaction可能まで待機する
        /// 
        /// </summary>
        private partial IEnumerator ActStandby();
        
        /// <summary>
        /// 一度空中に上がり、一定時間後に地面に落ちる攻撃
        /// 落下ごにdebrisの入れ替えとrespawnを行う
        /// </summary>
        private partial IEnumerator DropAttack();

        private MovementState state = new Standby();
        private bool destinationReached;
        private void Awake()
        {
            debrisSpawner.CheckNull();
            debrisSpawner.RandomSpawnDebris(30);
        }

        protected override IEnumerator Flow(IFlowBuilder context)
        {
            rb = GetComponent<Rigidbody>().CheckNull();
            agent = GetComponent<NavMeshAgent>().CheckNull();
            while (true)
            {
                yield return ActStandby();
                // NOTE: elifやwhenは使わずに、actionは条件が一致する場合、連続で実行される
                // 上から順に優先順位となっているが、状態によって調整したい場合は、logicを組む必要がある
                if (CanDropAttack())
                {
                    Debug.Log("落下攻撃");
                    yield return DropAttack();
                }
                
                else if (CanForwardBlow())
                {
                    Debug.Log("前方攻撃");
                    yield return ForwardBlow();
                }
                else if (CanThrowMines())
                {
                    Debug.Log("地雷を投げる");
                    yield return ThrowingMines();
                } 
                else if (CanChargeForward())
                {
                    Debug.Log("突進攻撃");
                    yield return ChargeForward();
                }
                
                yield return new WaitForSeconds(1f);
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private float keepStandStillTime = 0;
        private void FixedUpdate()
        {
            // 一定時間停止していたら再スタート
            if (agent.enabled && agent.velocity.sqrMagnitude < 0.01f)
            {
                keepStandStillTime += Time.fixedDeltaTime;
                if (keepStandStillTime > 7f && state is Standby or MoveToNearPlayerWithAgent or MoveToLeavePlayerWithAgent)
                {
                    keepStandStillTime = 0;
                    StartCoroutine(DelaySecondsRestartFlow(0.1f));
                }
            }
            else
            {
                keepStandStillTime = 0;
            }
        }

        public uint MaxHealth => 400;
        private uint currentHealth = 400;
        public uint CurrentHealth => currentHealth;
        public bool IsVisible => true;
        public event Action OnRemoveEvent;
        public void Dispose()
        {
            Destroy(gameObject);
            OnRemoveEvent?.Invoke();
        }

        public void OnDamage(uint damage, Vector3 hitPoint, Transform attacker)
        {
            Debug.Log($"OnDamage: {damage}");
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                Dispose();
            }
        }
    }
}