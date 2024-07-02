using System;
using UnityEngine;
using Script.Feature.Presenter;
namespace Script.Feature.View
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
                playerView.AttackDamege(other.gameObject);
            }
        }
    }
}