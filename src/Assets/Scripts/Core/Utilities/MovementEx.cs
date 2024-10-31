#region

using System.Collections;
using UnityEngine;

#endregion

namespace Core.Utilities
{
    public static class MovementEx
    {
        public static IEnumerator Knockback(this Transform transform, Vector3 direction, float strength, float duration)
        {
            var startTime = Time.time;
            while (Time.time < startTime + duration)
            {
                transform.position += direction.normalized * (strength * Time.deltaTime);
                yield return null;
            }
        }
    }
}