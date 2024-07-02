using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine;

namespace Script.Feature.View
{
    public class SwapView : MonoBehaviour
    {
        public float swapDistance;
        public float swapModeDuration;
        public float swapTimeScale;
        public float timeToReturnToNormal;
        public AnimationCurve returnCurve;
        public float resourceConsumptionOnEnter;
        public float resourceConsumptionPerSecond;
        public float resourceConsumptionOnSwap;

        public void UpdateUI()
        {
            // 必要に応じてUIの更新
        }

        public void HighlightSelectableObjects(GameObject[] selectableObjects)
        {
            foreach (GameObject obj in selectableObjects)
            {
                if (obj.CompareTag("SwappableObjects"))
                {
                    Renderer renderer = obj.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material.color = Color.red; // ハイライトとして黄色に変更
                    }
                }
            }
        }

        public void ApplySwapEffects(GameObject selectedObject)
        {
            // スワップ時のエフェクト
        }
    }
}