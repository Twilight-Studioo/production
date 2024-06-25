using UnityEngine;
namespace Script.Feature.Common
{
    [CreateAssetMenu(fileName = "CharacterParams.asset", menuName = "CharacterParams", order = 0)]
    public class CharacterParams :ScriptableObject
    {
        public int health = 1;

        public float speed = 1f;

        public float jumpPower = 1f;
        
        public int attackPower = 1;
    }
}