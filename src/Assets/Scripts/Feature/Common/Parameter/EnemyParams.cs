#region

using UnityEngine;

#endregion

namespace Feature.Common.Parameter
{
    public class EnemyParams : ScriptableObject
    {
        [Tooltip("発見距離")] public float foundDistance;

        [Tooltip("追跡(認識)距離")] public float pursuitDistance;

        [Tooltip("与ダメージ")] public uint damage;

        [Tooltip("HP")] public uint maxHp;

        [Tooltip("巡回中の移動速度")] public float patrolSpeed;

        [Tooltip("追跡中の移動速度")] public float pursuitSpeed;
    }
}