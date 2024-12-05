using UnityEngine;

namespace Core.Camera
{
    [System.Serializable]
    public class LookAtPoint
    {
        public float pathPosition;      // カメラのパス上の位置
        public Transform lookAtTarget;  // 対応するLookAtターゲット
    }
}