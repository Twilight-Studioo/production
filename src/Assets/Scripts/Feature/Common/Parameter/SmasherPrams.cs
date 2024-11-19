using Feature.Common.Constants;
using UnityEngine;
using UnityEngine.Serialization;

namespace Feature.Common.Parameter
{
    [CreateAssetMenu(fileName = "SmasherPrams.asset", menuName = "Params/SmasherPrams", order = 0)]

    public class SmasherPrams : EnemyParams
    {
        [Tooltip("攻撃の種類"), Header("攻撃の種類"),] 
        public SmasherAttackType attackType = SmasherAttackType.Charge;
        
        [Tooltip("突進する時間"), Header("突進時する時間"),] 
        public float chargeAttackTime = 5f;
        
        [Tooltip("突進の速度"), Header("突進の速度"),] 
        public float chargeSpeed = 5f;

        [Tooltip("チャージ時間"), Header("チャージ時間"),] 
        public float chargeTime = 2;

        [Tooltip("突進攻撃のダメージ"), Header("突進攻撃のダメージ"),]
        public uint chargeAttackDamage = 10;

        [Tooltip("突進攻撃後の待機時間"), Header("突進攻撃後の待機時間"),]
        public float chargeIntervalSec = 2;

        [Tooltip("アッパーの高さ"), Header("アッパーの高さ"),]
        public float upperHeight = 5;

        [Tooltip("アッパーのダメージ"), Header("アッパーのダメージ"),]
        public uint upperDamage = 10;

        [Tooltip("アッパー後の待機時間"), Header("アッパー後の待機時間"),]
        public float upperIntervalSec = 1;

        [Tooltip("平手打ちの当たる距離"), Header("平手打ちの当たる距離"),]
        public float slapDistance = 3;
        
        [Tooltip("平手打ちのダメージ"), Header("平手打ちのダメージ"),]
        public uint slapDamage = 3;

        [FormerlySerializedAs("strikingDistanceOfFallAttack"),FormerlySerializedAs("落下攻撃の当たる距離"),Tooltip("落下攻撃の当たる距離"), Header("落下攻撃の当たる距離"),]
        public float fallAttackDistance = 10;

        [Tooltip("落下速度"), Header("落下速度"),] 
        public float fallSpeed = 20;
        
        [Tooltip("落下攻撃のダメージ"), Header("落下攻撃のメージ"),]
        public uint fallAttackDamage = 10;
        
        [Tooltip("落下攻撃後の待機時間"), Header("落下攻撃後の待機時間"),]
        public float fallAttackIntervalSec = 1;

        [Tooltip("瓦礫のダメージ"), Header("瓦礫のダメージ"),]
        public uint debrisDamage = 5;
        
        [Tooltip("瓦礫のスピード"), Header("瓦礫のスピード"),]
        public float debrisSpeed = 5;
        
        [Tooltip("瓦礫攻撃後の待機時間"), Header("瓦礫攻撃後の待機時間"),]
        public float debrisAttackIntervalSec = 1;
    }

    public enum SmasherAttackType
    {
        Charge,
        Upper,
        None,
    }
}