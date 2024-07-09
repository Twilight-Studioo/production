using System;
using Feature.View;
using UnityEngine;
namespace Scripts.Feature.View
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