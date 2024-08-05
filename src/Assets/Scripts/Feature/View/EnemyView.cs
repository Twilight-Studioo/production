#region

using System;
using Core.Utilities.Health;
using Feature.Enemy;
using Feature.Interface;
using UnityEngine;

#endregion

namespace Feature.View
{
    public class EnemyView : MonoBehaviour, IHealthBar, IEnemy, IDamaged
    {
        private IEnemyAgent agent;
        
        private uint maxHealth;
        private uint currentHealth;

        public event Action OnDamageEvent;
        
        public event Action OnTakeDamageEvent;

        public void Execute()
        {
            agent = GetComponent<IEnemyAgent>();
            agent.FlowExecute();
        }

        public void OnDamage(uint damage, Vector3 hitPoint, Transform attacker)
        {
            OnDamageEvent?.Invoke();
            currentHealth -= damage;
        }
        
        public void SetHealth(uint health)
        {
            maxHealth = health;
            currentHealth = health;
        }

        public event Action OnDestroyEvent;
        public uint MaxHealth => maxHealth;
        public uint CurrentHealth => currentHealth;

        public bool IsVisible => true;
    }
}