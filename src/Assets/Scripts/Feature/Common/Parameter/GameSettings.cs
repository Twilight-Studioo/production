using UnityEngine;

namespace Feature.Common.Parameter
{
    [CreateAssetMenu(fileName = "GameSettings.asset", menuName = "GameSettings", order = 0)]
    public class GameSettings : ScriptableObject
    {
        public float cameraForwardOffsetFromPlayerMoved = 1.3f;
    }
}