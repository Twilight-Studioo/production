#region

using Feature.Interface;
using UnityEngine;

// ReSharper disable All

#endregion

namespace Feature.Component
{
    public class Slash : MonoBehaviour
    {
        private AudioSource audioSource;
        private uint damage;
        private AudioClip hitSound;

        private void OnTriggerEnter(Collider other)
        {
            var enemy = other.gameObject.GetComponent<IDamaged>()
                        ?? other.gameObject.GetComponentInParent<IDamaged>();
            if (enemy == null || other.gameObject.CompareTag("Player"))
            {
                return;
            }
            enemy.OnDamage(damage, other.transform.position, transform);
            if (enemy != null)
            {
                audioSource.PlayOneShot(hitSound);
            }
        }

        public void SetDamage(uint dmg, AudioClip selectedClip, AudioSource audioSource)
        {
            damage = dmg;
            hitSound = selectedClip;
            this.audioSource = audioSource;
        }
    }
}