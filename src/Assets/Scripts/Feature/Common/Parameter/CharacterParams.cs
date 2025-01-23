#region

using System;
using Core.Utilities.Parameter;
using UnityEngine;

#endregion

namespace Feature.Common.Parameter
{
    [CreateAssetMenu(fileName = "CharacterParams.asset", menuName = "CharacterParams", order = 0)]
    public class CharacterParams : BaseParameter
    {
        [ToggleGroup("BASE")] public float speed = 5f;

        public float jumpPower = 7f;


        [ToggleGroup("Health"), Tooltip("体力"),]
        public int health = 100;

        [Tooltip("ダメージ後の回復")] public uint damagedRecoveryHealth = 1;

        [Tooltip("敵を倒した時の回復量")] public uint killRecoveryHealth = 10;

        [Tooltip("スワップ後の回復する秒数")] public uint swappedRecoveryHealthTimeMillis = 1000;

        [Tooltip("スワップ後に回復する間隔")] public uint swappedRecoveryHealthIntervalMillis = 500;

        [Tooltip("スワップ後に回復する量")] public uint swappedRecoveryHealthQuantity = 1;

        [ToggleGroup("攻撃")] public int attackPower = 2;

        [Tooltip("〇秒以内で連続攻撃")] public float comboTimeWindow = 1f;

        [Tooltip("連続攻撃時の角度変化")] public float comboAngleOffset = 60f;

        [Tooltip("連続攻撃の最大回数(*数の+1の回数、2なら3コンボ)")]
        public float maxComboCount = 3;


        [Tooltip("攻撃のクールタイム")] public float attackCoolTime = 0.2f;

        [Tooltip("最大コンボ後のクールタイム")] public float maxComboCoolTime = 1.0f;
        // スワップ関連

        [ToggleGroup("スワップ"), Tooltip("スワップの最大継続時間(Milli)"), Space(10),]
        public float swapContinueMaxMillis = 4000f;

        [Tooltip("スワップ中のTimeScale"), Range(0.01f, 1.0f),]
        public float swapContinueTimeScale = 0.2f;

        [Tooltip("スワップ可能な最大距離"), Space(10),] public float canSwapDistance = 10.0f;

        [Tooltip("スワップ後何秒で元のタイムスケールに戻るか"), Space(10),]
        public uint swapReturnTimeMillis = 1000;

        [Tooltip("元のタイムスケールへの戻り方")] public SwapReturnCurve swapReturnCurve = SwapReturnCurve.Linear;

        // スタミナ関連

        [ToggleGroup("スタミナ"), Tooltip("スタミナを持つ最大値"),]
        public uint maxHasStamina = 12;

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

        [Tooltip("クナイを飛ばしたときのスタミナ消費")] public uint useDaggerUseStamina = 2;

        //ボルテージ関連
        [ToggleGroup("ボルテージ"), Tooltip("攻撃時に使うボルテージの量"),]
        public int useVoltageAttackValue = 50;

        [Tooltip("2段階目ボルテージ量")] public int votageTwoAttackValue = 65;
        [Tooltip("1回のスワップで増えるボルテージの量")] public int addVoltageSwapValue = 10;

        [Tooltip("ボルテージ使用時の攻撃上昇倍率")] public int voltageAttackPowerValue = 2;

        [Tooltip("ボルテージの最大値")] public int maxVoltage = 100;

        [Tooltip("ダメージ後の回復")] public uint damagedRecoveryVoltage = 5;

        [Tooltip("敵を倒した時の回復量(ボルテージ)")] public uint killRecoveryVoltage = 10;

        [Tooltip("スワップ後の回復する秒数(ボルテージ)")] public uint swappedRecoveryVoltageTimeMillis = 1000;

        [Tooltip("スワップ後に回復する間隔(ボルテージ)")] public uint swappedRecoveryVoltageIntervalMillis = 500;

        [Tooltip("スワップ後に回復する量(ボルテージ)")] public uint swappedRecoveryVoltageQuantity = 5;
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