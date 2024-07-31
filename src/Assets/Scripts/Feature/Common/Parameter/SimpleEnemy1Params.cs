#region

using UnityEngine;

#endregion

namespace Feature.Common.Parameter
{
    [CreateAssetMenu(fileName = "SimpleEnemy1Params.asset", menuName = "SimpleEnemy1Params", order = 0)]
    public class SimpleEnemy1Params : EnemyParams
    {
        [Tooltip("突撃前のdelay"), Header("突撃前のdelay"),]
        public float rushBeforeDelay = 0.2f;

        [Tooltip("突撃後のdelay"), Header("突撃後のdelay"),]
        public float rushAfterDelay = 0.6f;

        [Tooltip("突進開始距離"), Header("突進開始距離"),] public float rushStartDistance = 7f;

        [Tooltip("突進速度"), Header("突進速度"),] public float rushSpeed = 5f;
    }
}