#region

using Core.Utilities.Parameter;
using Feature.Common.Constants;
using UnityEngine;

#endregion

namespace Feature.Common.Parameter
{
    [CreateAssetMenu(fileName = "SimpleEnemy2FlyParams.asset", menuName = "Params/SimpleEnemy2FlyParams", order = 0)]
    public class SimpleEnemy2FlyParams : EnemyParams
    {
        [ToggleGroup("射撃"), Tooltip("射撃距離"),] public float shootDistance = 10f;

        [Tooltip("射撃スピード")] public float shootSpeed = 5f;

        [Tooltip("射撃間隔")] public float shootIntervalSec = 0.4f;

        [Tooltip("１度に打つ数"),] public int shootCount = 3;

        [Tooltip("射撃後の待機時間")] public float shootAfterSec = 1f;

        [Tooltip("射撃後に再度射撃可能になるまで")] public float shootCoolDownSec = 5f;

        [ToggleGroup("浮遊パラメーター"), Tooltip("浮遊速度"),]
        public float movePower = 2f;

        [Tooltip("最大高さ")] public float maxHeightFromGround = 20f;

        [Tooltip("天井まで最小距離")] public float minDistanceToCeiling = 5f;

        [Tooltip("目標距離")] public float distance = 5f;
    }
}