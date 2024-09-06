#region

using UnityEngine;

#endregion

namespace Feature.Common.Parameter
{
    [CreateAssetMenu(fileName = "PlayerViewParams.asset", menuName = "PlayerViewParams", order = 0)]
    public class PlayerViewParams : ScriptableObject
    {
        public float comboTimeWindow = 1f; // 〇秒以内の連続攻撃を許可
        public float comboAngleOffset = 50f; // 連続攻撃時の角度変化
        public int maxComboCount = 2; // 連続攻撃の最大回数
    }
}