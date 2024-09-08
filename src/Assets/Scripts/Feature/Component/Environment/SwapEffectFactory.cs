using Core.Utilities.Effect;
using UnityEngine;

namespace Feature.Component.Environment
{
    public class SwapEffectFactory: MonoBehaviour
    {
        [SerializeField] private GameObject effectPrefab; // 使用するエフェクトのPrefab
        private const float EffectLifetime = 1.0f; // エフェクトの寿命
        private const int InitialPoolSize = 3; // 初期のプールサイズ
        private const float CheckInterval = 5.0f; // 非アクティブなエフェクトをチェックする間隔

        private EffectManager<VFXView> effectManager;

        private void Awake()
        {
            var swapView = effectPrefab.GetComponent<VFXView>();
            if (swapView == null)
            {
                Debug.LogError("Effect prefab must have SwapView component.");
                return;
            }
            effectManager = new (InitialPoolSize, CheckInterval, this);
            effectManager.InitializePool(swapView);
        }

        public void PlayEffectAtPosition(Vector3 position)
        {
            effectManager.PlayEffect(position, effectPrefab.GetComponent<VFXView>(), EffectLifetime);
        }
    }
}