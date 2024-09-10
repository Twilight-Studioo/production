#region

using Core.Utilities;
using Feature.Interface;
using UnityEngine;

#endregion

namespace Feature.Component.Environment
{
    public class PlayerStart : MonoBehaviour
    {
        [SerializeField] private GameObject playerRef;

        private bool isPlayerSpawned;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, new(0.4f, 0.7f, 0.4f));
            Gizmos.DrawWireSphere(Vector3.zero, 2);
        }

        public IPlayerView OnStart()
        {
            if (isPlayerSpawned)
            {
                return null;
            }

            isPlayerSpawned = true;
            var player = ObjectFactory.CreateObject(playerRef, transform.position, Quaternion.identity);
            return player.GetComponent<IPlayerView>();
        }
    }
}