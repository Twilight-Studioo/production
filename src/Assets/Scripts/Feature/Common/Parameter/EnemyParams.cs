#region

using UnityEngine;

#endregion

namespace Feature.Common.Parameter
{
    public class EnemyParams : ScriptableObject
    {
        [Tooltip("発見距離")] public float foundDistance = 10f;

        [Tooltip("追跡(認識)距離")] public float pursuitDistance = 14f;

        [Tooltip("与ダメージ")] public uint damage = 10;

        [Tooltip("HP")] public uint maxHp = 100;

        [Tooltip("巡回中の移動速度")] public float patrolSpeed = 1f;

        [Tooltip("追跡中の移動速度")] public float pursuitSpeed = 1.2f;
    }
}