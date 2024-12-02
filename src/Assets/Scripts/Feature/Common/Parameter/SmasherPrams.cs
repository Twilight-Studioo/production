using Feature.Common.Constants;
using UnityEngine;

namespace Feature.Common.Parameter
{
    [CreateAssetMenu(fileName = "SmasherPrams.asset", menuName = "Params/SmasherPrams", order = 0)]

    public class SmasherPrams : EnemyParams
    {
        [Tooltip("体力")]
        public uint health  = 100;
        
        [Tooltip("突進する時間")]
        public float chargeAttackTime = 0.5f;
        
        [Tooltip("突進の速度")] 
        public float chargeSpeed = 500f;

        [Tooltip("チャージ時間")] 
        public float chargeTime = 3f;

        [Tooltip("突進攻撃のダメージ"),]
        public uint chargeAttackDamage = 10;

        [Tooltip("突進攻撃後の待機時間")]
        public float chargeIntervalSec = 0.3f;

        [Tooltip("アッパーの高さ")]
        public float upperHeight = 400f;
        
        [Tooltip("アッパーの発生時間")] 
        public float upperOccurrenceTime = 0.5f;

        [Tooltip("アッパーのダメージ")]
        public uint upperDamage = 10;

        [Tooltip("アッパー後の待機時間")]
        public float upperIntervalSec = 0.5f;

        [Tooltip("ジャンプの発生時間")] 
        public float jumpOccurrenceTime = 0.5f;
        
        [Tooltip("ジャンプ後の待機時間")] 
        public float jumpIntervalSec = 0.5f;

        [Tooltip("平手打ちの当たる距離")]
        public float slapDistance = 3f;
        
        [Tooltip("平手打ちのダメージ")]
        public uint slapDamage = 3;

        [Tooltip("平手打ちの発生時間")]
        public float slapOccurrenceTime = 0.1f;
        
        [Tooltip("平手打ち後の待機時間")]
        public float slapIntervalSec = 0.1f;

        [Tooltip("落下攻撃の当たる距離")]
        public float fallAttackDistance = 20f;

        [Tooltip("落下速度")] 
        public float fallSpeed = 200f;
        
        [Tooltip("落下攻撃のダメージ")]
        public uint fallAttackDamage = 10;
        
        [Tooltip("落下攻撃の発生時間")]
        public float fallAttackOccurrenceTime = 0.1f;
        
        [Tooltip("落下攻撃後の待機時間")]
        public float fallAttackIntervalSec = 1f;

        [Tooltip("瓦礫のダメージ")]
        public uint debrisDamage = 5;
        
        [Tooltip("瓦礫のスピード")]
        public float debrisSpeed = 30f;
        
        [Tooltip("瓦礫攻撃後の待機時間")]
        public float debrisAttackOccurrenceTime = 1f;
        
        [Tooltip("瓦礫攻撃後の待機時間")]
        public float debrisAttackIntervalSec = 1f;
        
        [Tooltip("地雷発射後の待機時間")]
        public float mineOccurrenceTime  = 1f;
        
        [Tooltip("地雷発射後の待機時間")]
        public float mineIntervalSec = 10f;
        
        [Tooltip("地雷発射の速度(縦)")]
        public uint mineSpeedVertical = 50;
        
        [Tooltip("地雷発射の速度(横)"),]
        public uint mineSpeedBeside = 25;
        
        [Tooltip("地雷のダメージ")]
        public uint mineDamage = 10;
    }
}