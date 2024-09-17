#region

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Core.Utilities.Effect
{
    public class EffectManager<T> where T : MonoBehaviour
    {
        private readonly Queue<EffectInstance<T>> activeEffects = new(); // 一時的にアクティブなエフェクト
        private readonly MonoBehaviour context; // コルーチンの実行コンテキスト
        private readonly Queue<EffectInstance<T>> effectPool = new(); // プールされたエフェクト
        private readonly float inactiveCheckInterval;
        private readonly float maxIdleTime = 10.0f; // エフェクトが使われなければ破棄するまでの時間
        private readonly int poolSize;
        private readonly int useThresholdForExpansion = 3; // プールを拡張するための使用回数の閾値

        public EffectManager(int initialPoolSize, float checkInterval, MonoBehaviour context)
        {
            poolSize = initialPoolSize;
            inactiveCheckInterval = checkInterval;
            this.context = context;
        }

        public void InitializePool(T prefab)
        {
            for (var i = 0; i < poolSize; i++)
            {
                var effectObject = Object.Instantiate(prefab);
                effectObject.enabled = false;
                effectPool.Enqueue(new(effectObject));
            }

            context.InvokeRepeating(nameof(CheckAndRemoveInactiveEffects), inactiveCheckInterval,
                inactiveCheckInterval);
        }

        public T PlayEffect(Vector3 position, T prefab, float lifetime)
        {
            EffectInstance<T> effectInstance;

            if (effectPool.Count > 0)
            {
                effectInstance = effectPool.Dequeue();
            }
            else if (activeEffects.Count > 0)
            {
                effectInstance = activeEffects.Dequeue();
            }
            else
            {
                var effectObject = Object.Instantiate(prefab);
                effectInstance = new(effectObject);
            }

            effectInstance.Effect.transform.position = position;
            effectInstance.Effect.enabled = true;
            effectInstance.UpdateUsage();

            activeEffects.Enqueue(effectInstance);
            context.StartCoroutine(ReturnEffectToPool(effectInstance, lifetime));

            return effectInstance.Effect;
        }

        private IEnumerator ReturnEffectToPool(EffectInstance<T> effectInstance, float delay)
        {
            yield return new WaitForSeconds(delay);
            effectInstance.Effect.enabled = false;

            if (effectInstance.UsageCount >= useThresholdForExpansion)
            {
                // 使用回数が閾値を超えた場合はプールを拡張
                effectPool.Enqueue(effectInstance);
            }
            else
            {
                // プールに戻す
                activeEffects.Enqueue(effectInstance);
            }
        }

        private void CheckAndRemoveInactiveEffects()
        {
            var initialCount = activeEffects.Count;
            for (var i = 0; i < initialCount; i++)
            {
                var effectInstance = activeEffects.Dequeue();
                if (Time.time - effectInstance.LastUsedTime > maxIdleTime)
                {
                    // 最後に使われてから一定時間が経過したエフェクトを破棄
                    Object.Destroy(effectInstance.Effect.gameObject);
                }
                else
                {
                    // まだ使用される可能性があるのでキューに戻す
                    activeEffects.Enqueue(effectInstance);
                }
            }
        }
    }
}