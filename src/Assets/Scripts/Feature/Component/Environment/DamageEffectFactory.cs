using System;
using Core.Utilities;
using Core.Utilities.Effect;
using UnityEngine;

namespace Feature.Component.Environment
{
    public class DamageEffectFactory : MonoBehaviour
    {
        public enum Type
        {
            Player,
            Enemy,
            EnemyKill,
        }

        private const float EffectLifetime = 1.0f; // エフェクトの寿命
        private const int InitialPoolSize = 3; // 初期のプールサイズ
        private const float CheckInterval = 5.0f; // 非アクティブなエフェクトをチェックする間隔
        [SerializeField] private GameObject playerDamageEffectPrefab;
        [SerializeField] private GameObject enemyDamageEffectPrefab;
        private EffectManager<VFXView> enemyDamageEffectManager;

        private EffectManager<VFXView> playerDamageEffectManager;

        private void Awake()
        {
            playerDamageEffectPrefab.CheckNull();
            playerDamageEffectManager = new(InitialPoolSize, CheckInterval, this);
            playerDamageEffectManager.InitializePool(playerDamageEffectPrefab.GetComponent<VFXView>().CheckNull());
            enemyDamageEffectPrefab.CheckNull();
            enemyDamageEffectManager = new(InitialPoolSize, CheckInterval, this);
            enemyDamageEffectManager.InitializePool(enemyDamageEffectPrefab.GetComponent<VFXView>().CheckNull());
        }

        public void PlayEffectAtPosition(Vector3 position, Quaternion rotation, Type type)
        {
            switch (type)
            {
                case Type.Player:

                    playerDamageEffectManager.PlayEffect(position, playerDamageEffectPrefab.GetComponent<VFXView>(),
                        EffectLifetime, rotation);
                    break;
                case Type.Enemy:

                    enemyDamageEffectManager.PlayEffect(position, enemyDamageEffectPrefab.GetComponent<VFXView>(),
                        EffectLifetime, rotation);
                    break;
                case Type.EnemyKill:
                    enemyDamageEffectManager
                        .PlayEffect(position, enemyDamageEffectPrefab.GetComponent<VFXView>(), EffectLifetime, rotation,
                            view => { view.GetEffect().CheckNull()?.SetBool("Death", true); });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}