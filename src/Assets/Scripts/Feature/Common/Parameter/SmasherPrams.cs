using Feature.Common.Constants;
using UnityEngine;

namespace Feature.Common.Parameter
{
    [CreateAssetMenu(fileName = "SmasherPrams.asset", menuName = "Params/SmasherPrams", order = 0)]

    public class SmasherPrams : EnemyParams
    {
        [Tooltip("攻撃の種類"), Header("攻撃の種類"),] public SmasherAttackType attackType = SmasherAttackType.Charge;
        
        [Tooltip("突進距離"), Header("突進距離"),] public float chargeDistance = 5f;

        [Tooltip("突進攻撃のダメージ"), Header("突進攻撃のダメージ"),]
        public float chargeAttackDamage = 10;

        [Tooltip("突進攻撃後の待機時間"), Header("突進攻撃後の待機時間"),]
        public float chargeIntervalSec = 2;

        [Tooltip("アッパーの高さ"), Header("アッパーの高さ"),]
        public float upperHeight = 5;

        [Tooltip("アッパーのダメージ"), Header("アッパーのダメージ"),]
        public float upperDamage = 10;

        [Tooltip("アッパー後の待機時間"), Header("アッパー後の待機時間"),]
        public float upperIntervalSec = 1;

    }

    public enum SmasherAttackType
    {
        Charge,
        Upper,
        None,
    }
}