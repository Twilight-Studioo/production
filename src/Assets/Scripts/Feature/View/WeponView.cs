#region

using UnityEngine;

#endregion

namespace Feature.View
{
    public class WeponView : MonoBehaviour
    {
        private PlayerView playerView;

        private void Start()
        {
            playerView = GetComponentInParent<PlayerView>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                Destroy(other.gameObject);
            }
        }
    }
}