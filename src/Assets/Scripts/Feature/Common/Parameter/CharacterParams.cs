#region

using System;
using UnityEngine;

#endregion

namespace Feature.Common.Parameter
{
    [CreateAssetMenu(fileName = "CharacterParams.asset", menuName = "CharacterParams", order = 0)]
    public class CharacterParams : ScriptableObject
    {
        public int health = 100;

        public float speed = 5f;

        public float jumpPower = 7f;

        public int attackPower = 2;
        public float comboTimeWindow = 1f; // 〇秒以内の連続攻撃を許可
        public float comboAngleOffset = 60f; // 連続攻撃時の角度変化
        public int maxComboCount = 3; // 連続攻撃の最大回数
        
        // スワップ関連

        [Tooltip("スワップの最大継続時間(Milli)"), Space(10),]
        public float swapContinueMaxMillis = 4000f;

        [Tooltip("スワップ中のTimeScale"), Range(0.01f, 1.0f),]
        public float swapContinueTimeScale = 0.2f;

        [Tooltip("スワップ可能な最大距離"), Space(10),] public float canSwapDistance = 10.0f;

        [Tooltip("スワップ後何秒で元のタイムスケールに戻るか"), Space(10),]
        public uint swapReturnTimeMillis = 1000;

        [Tooltip("元のタイムスケールへの戻り方")] public SwapReturnCurve swapReturnCurve = SwapReturnCurve.Linear;

        // スタミナ関連

        [Tooltip("スタミナを持つ最大値"), Space(5),] public uint maxHasStamina = 12;

        [Tooltip("スワップモードに入ったときのスタミナ消費量"), Space(10),]
        public uint enterSwapUseStamina = 3;

        [Tooltip("スワップモード中何秒ごとにスタミナを消費するか"), Space(10),]
        public uint swapModeStaminaUsageIntervalMillis = 1000;

        [Tooltip("スワップモード中に継続して消費するスタミナ")] public uint swapModeStaminaUsage = 1;

        [Tooltip("スワップした時に消費するスタミナ"), Space(10),]
        public uint swapExecUseStamina = 3;

        [Tooltip("スワップして何秒で回復し始めるか"), Space(10),]
        public uint recoveryTimeMillis = 800;

        [Tooltip("1回の回復で増えるスタミナの量")] public uint resourceRecoveryQuantity = 1;

        [Tooltip("何ミリ秒ごとにリソースが回復するか")] public uint recoveryStaminaTimeMillis = 800;
        
        //ボルテージ関連
        [Tooltip("攻撃時に使うボルテージの量")] 
        public int useVoltageAttackValue = 50;
            
        [Tooltip("1回のスワップで増えるボルテージの量")] 
        public int addVoltageSwapValue = 10;
                
        [Tooltip("ボルテージ使用時の攻撃上昇倍率")] 
        public int voltageAttackPowerValue = 2;

        [Tooltip("クナイを飛ばしたときのスタミナ消費")] public uint useDaggerUseStamina = 2;

        [Tooltip("攻撃方向への移動量")] public float snapPower = 4f;
    }

    [Serializable]
    public enum SwapReturnCurve
    {
        Linear,
        EaseIn,
        EaseOut,
        EaseInOut,
    }
}