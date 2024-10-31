#region

using Feature.Common.Constants;
using UnityEngine;

#endregion

namespace Feature.Common.Parameter
{
    [CreateAssetMenu(fileName = "DroneParams.asset", menuName = "Params/DroneParams", order = 0)]
    public class DroneParams : EnemyParams
    {
        [Tooltip("水平距離でのプレイヤーとの距離の目標")] public float playerKeepDistance = 8f;

        [Header("自爆"), Tooltip("自爆を開始するまでのHPのしきい値"),]
        public float thresholdHealth = 4f;

        [Tooltip("自爆までのカウントダウン時間")] public float selfDestructionCountDownSec = 3f;

        [Tooltip("自爆時の移動速度")] public float selfDestructionMoveSpeed = 5f;

        [Tooltip("自爆の爆発半径")] public float explosionRadius = 5f;

        [Tooltip("自爆のダメージ")] public uint explosionDamage = 20;

        [Header("攻撃の種類"), Tooltip("攻撃の種類"),] public DroneAttackType attackType = DroneAttackType.Bullet;

        [Header("Bullet"), Tooltip("弾のプレハブ"),] public GameObject bulletPrefab;

        [Tooltip("弾の速度")] public float bulletSpeed = 4f;

        [Tooltip("一度に発射する弾の数")] public int shotCount = 1;

        [Tooltip("弾の発射後の待機時間")] public float shootIntervalSec = 1f;

        [Header("Ray"), Tooltip("ビームのダメージ"),] public uint rayDamage = 10;

        [Tooltip("ビームの射程")] public float rayRange = 7f;

        [Tooltip("ビーム前の待機時間")] public float rayWaitSec = 1f;

        [Tooltip("ビーム後の待機時間")] public float rayIntervalSec = 1f;

        // MARK: unused
        // [Tooltip("ビームの幅")]
        // public float rayWidth = 1f;
    }

    public enum DroneAttackType
    {
        Bullet,
        Ray,
        None,
    }
}