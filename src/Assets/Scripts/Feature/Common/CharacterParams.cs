#region

using UnityEngine;

#endregion

namespace Feature.Common
{
    [CreateAssetMenu(fileName = "CharacterParams.asset", menuName = "CharacterParams", order = 0)]
    public class CharacterParams : ScriptableObject
    {
        public int health = 1;

        public float speed = 5f;

        public float jumpPower = 7f;

        public int attackPower = 2;


        /// <summary>
        ///     スワップの最大継続時間(Milli)
        /// </summary>
        public float swapContinueMaxMillis = 4000f;

        /// <summary>
        ///     スワップ中のTimeScale
        /// </summary>
        [Range(0.01f, 1.0f)] public float swapContinueTimeScale = 0.2f;

        /// <summary>
        ///     リソースを持つ最大値
        /// </summary>
        public uint maxHasResource = 12;

        /// <summary>
        ///     何ミリ秒ごとにリソースが回復するか
        /// </summary>
        public uint recoveryResourceTimeMillis = 800;

        /// <summary>
        ///     スワップ可能な最大距離
        /// </summary>
        public float canSwapDistance = 10.0f;

        /// <summary>
        ///     スワップモード中に継続して消費するリソース
        /// </summary>
        public uint swapContinuedUseResource = 1;

        /// <summary>
        ///     スワップモード中何秒ごとにリソースを消費するか
        /// </summary>
        public uint swapContinueUsageTimeMillis = 1000;

        /// <summary>
        ///     スワップした時に消費するリソース
        /// </summary>
        public uint swapExecUseResource = 3;

        /// <summary>
        ///     1回の回復で増えるリソースの量
        /// </summary>
        public uint resourceRecoveryQuantity = 1;
    }
}