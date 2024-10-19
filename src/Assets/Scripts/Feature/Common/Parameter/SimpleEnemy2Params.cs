#region

using Feature.Common.Constants;
using UnityEngine;

#endregion

namespace Feature.Common.Parameter
{
    [CreateAssetMenu(fileName = "SimpleEnemy2Params.asset", menuName = "Params/SimpleEnemy2Params", order = 0)]
    public class SimpleEnemy2Params : EnemyParams
    {
        [Tooltip("射撃距離"), Header("射撃距離"),] public float shootDistance = 15f;

        [Tooltip("射撃スピード"), Header("射撃スピード"),] public float shootSpeed = 5f;

        [Tooltip("射撃間隔"), Header("射撃間隔"),] public float shootIntervalSec = 0.4f;

        [Tooltip("１度に打つ数"), Header("１度に打つ数"),] public int shootCount = 3;

        [Tooltip("射撃後の待機時間"), Header("射撃後の待機時間"),]
        public float shootAfterSec = 1f;
    }
}