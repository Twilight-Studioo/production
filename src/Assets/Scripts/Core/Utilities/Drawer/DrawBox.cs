#region

using UnityEngine;

#endregion

namespace Core.Utilities.Drawer
{
    public class DrawBox : MonoBehaviour
    {
        [SerializeField] private Color color = Color.red;
        [SerializeField] private Vector3 halfExtents = Vector3.one;

        private void OnDrawGizmos()
        {
            Gizmos.color = color;
            Gizmos.DrawWireCube(transform.position, halfExtents * 2);
        }
    }
}