using UnityEngine;

namespace Core.Utilities
{
    public static class Rays
    {
        
        public static float GetGroundDistance(this Transform transform, float maxDistance)
        {
            return Physics.Raycast(transform.position, Vector3.down, out var hit, maxDistance) ? hit.distance : maxDistance;
        }
        
        public static float GetDirectionDistance(this Transform transform, Vector3 direction, float maxDistance)
        {
            return Physics.Raycast(transform.position, direction, out var hit, maxDistance) ? hit.distance : maxDistance;
        }
    }
}