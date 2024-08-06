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
            CurrentHealth -= damage;
        }

        public void SetHealth(uint health)
        {
            MaxHealth = health;
            CurrentHealth = health;
        }

        public event Action OnDestroyEvent;
        public uint MaxHealth { get; private set; }

        public uint CurrentHealth { get; private set; }

        public bool IsVisible => true;
    }
}