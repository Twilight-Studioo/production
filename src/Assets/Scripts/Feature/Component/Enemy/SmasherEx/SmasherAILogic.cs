using System;
using System.Collections;
using Core.Utilities;
using Feature.Component.Enemy.SmasherEx.State;
using Feature.Interface;
using UniRx;
using UnityEngine;

namespace Feature.Component.Enemy.SmasherEx
{
    public partial class SmasherAI
    {
        private IDisposable chargeForwardDisposable;
        private Mine spawnedMine;

        /// <summary>
        ///     突進攻撃が可能か
        /// </summary>
        private partial bool CanChargeForward();

        /// <summary>
        ///     地雷を投げることが可能か
        /// </summary>
        private partial bool CanThrowMines();

        /// <summary>
        ///     前方に向かって攻撃が可能か
        /// </summary>
        private partial bool CanForwardBlow();

        /// <summary>
        ///     落下攻撃が可能か
        /// </summary>
        private partial bool CanDropAttack();

        private partial Vector3 CalculateVelocity(GameObject mine, Vector3 target, float angle = Angle,
            float velocityMultiplier = 1.0f, bool useGravity = true);

        /// <summary>
        ///     アクションを起こせるまでスタンバイ
        /// </summary>
        private partial IEnumerator ActStandby()
        {
            state = MovementState.Standby;
            var player = ObjectFactory.Instance.FindPlayer();
            while (
                player == null ||
                Vector3.Distance(transform.position, player.transform.position) > MaxFindPlayerDistance ||
                (!CanChargeForward() && !CanThrowMines() && !CanForwardBlow() && !CanDropAttack())
            )
            {
                yield return new WaitForFixedUpdate();
                player = ObjectFactory.Instance.FindPlayer();
                transform.LookAt(player.transform.position);
                // 何もない時は移動
                if (state is Standby)
                {
                    MoveToPlayerWithAgent();
                }

                if (agent != null && agent.enabled)
                {
                    if (agent.remainingDistance <= agent.stoppingDistance)
                    {
                        if (!destinationReached)
                        {
                            if (!agent.hasPath || agent.velocity.sqrMagnitude <= 0.1f)
                            {
                                // 到達した場合は再スタートをかけてリセットする
                                destinationReached = true;
                                state = MovementState.Standby;
                                StartCoroutine(DelaySecondsRestartFlow(0.1f));
                            }
                        }
                    }
                    else
                    {
                        destinationReached = false;
                    }
                }
            }
        }

        // 地雷を投げる
        private partial IEnumerator ThrowingMines()
        {
            state = MovementState.ThrowingMines;
            if (spawnedMine != null && spawnedMine.isActiveAndEnabled)
            {
                Destroy(spawnedMine.gameObject);
                spawnedMine = null;
            }

            var target = ObjectFactory.Instance.FindPlayer()?.transform;
            if (target == null)
            {
                yield break;
            }

            var mine = ObjectFactory.Instance.CreateObject(minePrefab, transform.position + Vector3.up * 1f,
                Quaternion.identity);
            var bRigidbody = mine.GetComponent<Rigidbody>();
            var direction = (target.position - mine.transform.position).normalized;

            // 開始地点と目標地点
            var targetPosition = target.position + Vector3.up + direction * 2f;

            // 初速度を計算
            var velocity = CalculateVelocity(mine, targetPosition, velocityMultiplier: 1.5f);

            // Rigidbodyに力を加える
            bRigidbody.velocity = velocity;
            spawnedMine = mine.GetComponent<Mine>().CheckNull();
            spawnedMine.OnDestroyed += () => spawnedMine = null;
            Observable.Timer(TimeSpan.FromSeconds(2f))
                .Subscribe(_ => spawnedMine.Resume(transform))
                .AddTo(this);
        }

        private partial IEnumerator ChargeForward()
        {
            state = MovementState.ChargeForward;
            Debug.Log("チャージ中");
            // 一定時間待機
            yield return new WaitForSeconds(0.8f);
            // 突進速度
            const float chargeSpeed = 9f;
            // Rayの長さ
            const float rayLength = 1.5f;

            while (true)
            {
                yield return new WaitForFixedUpdate();
                if (spawnedMine == null || !spawnedMine.isActiveAndEnabled)
                {
                    spawnedMine = null;
                    break;
                }

                // 自機が地雷に接近すると自爆する
                var finished = Vector3.Distance(transform.position, spawnedMine.transform.position) < 2f;
                if (finished)
                {
                    spawnedMine.LaunchExplosionForSelf();
                    break;
                }

                // playerが地雷との間に存在しなくなったら中断
                // FIXME: 地雷と一定距離orSpeedなら止まれない
                var player = ObjectFactory.Instance.FindPlayer();
                var toMineDistance = Vector3.Distance(transform.position, spawnedMine.transform.position);
                if (toMineDistance > 7f && !ExistsBetweenTwoPoints(transform.position, spawnedMine.transform.position,
                        player.transform))
                {
                    break;
                }

                ChargingForwardTick(chargeSpeed, rayLength);
            }

            StartCoroutine(DelaySecondsRestartFlow(1f));
            yield return null;
        }

        private partial IEnumerator ForwardBlow()
        {
            state = MovementState.ForwardBlow;
            var player = ObjectFactory.Instance.FindPlayer().CheckNull();
            while (ToPlayerDistance() > 1.5f)
            {
                yield return new WaitForFixedUpdate();
                transform.LookAt(player.transform.position);
                agent.SetDestination(player.transform.position);
            }

            var hits = transform.GetBoxForwardCastAll(new(1f, 1f, 1f), 1f, 10);
            foreach (var raycastHit in hits)
            {
                if (raycastHit.transform == null || raycastHit.transform == transform)
                {
                    continue;
                }

                var damaged = raycastHit.transform.GetComponent<IDamaged>();
                if (damaged != null)
                {
                    damaged.OnDamage(15, raycastHit.point, transform);
                    continue;
                }

                var component = raycastHit.transform.GetComponent<Rigidbody>();
                if (component != null)
                {
                    var dir = (raycastHit.transform.position - transform.position).normalized;
                    dir.y += 0.7f;
                    component.velocity = dir * 3f;
                }
            }

            state = MovementState.Standby;
            yield return null;
        }

        /// <summary>
        ///     一度空中に上がり、一定時間後に地面に落ちる攻撃
        ///     落下後にdebrisの入れ替えとrespawnを行う
        /// </summary>
        private partial IEnumerator DropAttack()
        {
            // 空中に上昇 (この時点でのplayerの真上に向かう)
            var player = ObjectFactory.Instance.FindPlayer().CheckNull();
            var target = player.transform.position + Vector3.up * 8f;
            var maxHeight = debrisSpawner.transform.position.y - 1f;
            target.y = target.y > maxHeight ? maxHeight : target.y;
            agent.enabled = false;
            rb.isKinematic = false;
            while (true)
            {
                if (Vector3.Distance(transform.position, target) < 1f)
                {
                    break;
                }

                yield return new WaitForFixedUpdate();
                var direction = (target - transform.position).normalized;
                rb.velocity = direction * 15f;
            }

            rb.velocity = Vector3.zero;
            yield return new WaitForSeconds(0.6f);
            rb.useGravity = false;
            rb.velocity = Physics.gravity * 4f;
            while (transform.GetGroundDistance(10f) > 1.5f)
            {
                yield return new WaitForFixedUpdate();
            }

            rb.velocity = Vector3.zero;
            rb.useGravity = true;
            Debug.Log("着地");
            rb.isKinematic = true;
            agent.enabled = true;
            yield return new WaitForFixedUpdate();
            var hits = transform.GetBoxCastAll(new(50f, 1f, 1f), Vector3.up, 0.5f, 100,
                LayerMask.GetMask("Default", "NotHitAreaObject", "Character"));
            foreach (var raycastHit in hits)
            {
                if (raycastHit.transform == null)
                {
                    continue;
                }

                var damaged = raycastHit.transform.GetComponent<IDamaged>();
                if (damaged != null)
                {
                    damaged.OnDamage(10, raycastHit.point - new Vector3(0f, 1f, 0f), transform);
                    continue;
                }

                var component = raycastHit.transform.GetComponent<Rigidbody>();
                if (component != null)
                {
                    component.velocity = Vector3.up * 3f;
                }
            }

            debrisSpawner.RandomLifeTimeDestroy(0.6f);
            debrisSpawner.RandomSpawnDebris(20);
            yield return new WaitForSeconds(2f);
        }
    }
}