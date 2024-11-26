#region

using UnityEngine;

#endregion

namespace Core.Utilities
{
    public static class Rays
    {
        public static float GetGroundDistance(this Transform transform, float maxDistance) =>
            Physics.Raycast(transform.position, Vector3.down, out var hit, maxDistance) ? hit.distance : maxDistance;

        public static float GetDirectionDistance(this Transform transform, Vector3 direction, float maxDistance) =>
            Physics.Raycast(transform.position, direction, out var hit, maxDistance) ? hit.distance : maxDistance;

        public static RaycastHit[] GetBoxCastAll(this Transform transform, Vector3 halfExtents, Vector3 direction,
            float maxDistance, int capacity = 5, LayerMask layerMask = default)
        {
            var results = new RaycastHit[capacity];
            Physics.BoxCastNonAlloc(transform.position, halfExtents, direction, results, Quaternion.identity,
                maxDistance, layerMask);
            GizmoManager.Instance?.RequestGizmo(transform.position, halfExtents, direction, maxDistance, 1f);
            return results;
        }

        public static RaycastHit[] GetBoxForwardCastAll(this Transform transform, Vector3 halfExtents,
            float maxDistance, int capacity = 5) =>
            GetBoxCastAll(transform, halfExtents, transform.forward, maxDistance, capacity);

        public static RaycastHit[] GetCircleCastAll(this Transform transform, float radius, Vector3 direction,
            float maxDistance, int capacity = 5)
        {
            var results = new RaycastHit[capacity];
            Physics.SphereCastNonAlloc(transform.position, radius, direction, results, maxDistance);
            return results;
        }
    }
}