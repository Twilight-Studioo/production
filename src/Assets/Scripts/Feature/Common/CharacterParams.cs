#region

using UnityEngine;

#endregion

namespace Feature.Common
{
    [CreateAssetMenu(fileName = "CharacterParams.asset", menuName = "CharacterParams", order = 0)]
    public class CharacterParams : ScriptableObject
    {
        public int health = 1;

        public float speed = 1f;

        public float jumpPower = 1f;

        public int attackPower = 1;

        public float swapContinueMaxSec = 4f;

        [Range(0.01f, 1.0f)] public float swapContinueTimeScale = 0.2f;

        public uint swapUsedResource = 6;

        public uint maxHasResource = 12;

        public uint secondOfRecoveryResource = 1;

        public float canSwapDistance = 10.0f;
    }
}