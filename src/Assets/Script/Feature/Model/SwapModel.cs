using UnityEngine;
namespace Script.Feature.Model
{
    using UnityEngine;

    public class SwapModel
    {
        public float SwapDistance { get; set; }
        public float SwapModeDuration { get; set; }
        public float SwapTimeScale { get; set; }
        public float TimeToReturnToNormal { get; set; }
        public AnimationCurve ReturnCurve { get; set; }
        public float ResourceConsumptionOnEnter { get; set; }
        public float ResourceConsumptionPerSecond { get; set; }
        public float ResourceConsumptionOnSwap { get; set; }

        public float CurrentResource { get; set; }

        public SwapModel()
        {
            // 初期値の設定
            SwapDistance = 10f;
            SwapModeDuration = 5f;
            SwapTimeScale = 0.5f;
            TimeToReturnToNormal = 0.2f;
            ReturnCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
            ResourceConsumptionOnEnter = 10f;
            ResourceConsumptionPerSecond = 1f;
            ResourceConsumptionOnSwap = 5f;
            CurrentResource = 100f; // リソースの初期値
        }
    }

}