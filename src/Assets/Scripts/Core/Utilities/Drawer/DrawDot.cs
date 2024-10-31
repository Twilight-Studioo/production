#region

using UnityEngine;

#endregion

namespace Core.Utilities.Drawer
{
    public class DrawDot : MonoBehaviour
    {
        [SerializeField] private Color color = Color.red;
        [SerializeField] private float radius = 0.2f;

        private void OnDrawGizmos()
        {
            Gizmos.color = color;
            Gizmos.DrawSphere(transform.position, radius);
        }
    }
}