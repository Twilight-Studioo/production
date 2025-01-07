#region

using System;
using Feature.Interface;
using UnityEngine;

// ReSharper disable All

#endregion

namespace Feature.Component
{
    public class Slash : MonoBehaviour
    {
        private uint damage;
        
        public event Action<DamageResult> OnHitEvent;

        private void OnTriggerEnter(Collider other)
        {
            var enemy = other.gameObject.GetComponent<IDamaged>()
                        ?? other.gameObject.GetComponentInParent<IDamaged>();
            if (enemy == null || other.gameObject.CompareTag("Player"))
            {
                return;
            }
            var result = enemy.OnDamage(damage, other.transform.position, transform);
            OnHitEvent?.Invoke(result);
        }

        public void SetDamage(uint dmg)
        {
            damage = dmg;
        }
    }
}