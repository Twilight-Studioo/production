#region

using System;
using Core.Utilities.Health;
using Feature.Common.Constants;
using Feature.Interface;
using UnityEngine;

#endregion

namespace Feature.View
{
    public class EnemyView : MonoBehaviour, IHealthBar, IEnemy, IDamaged
    {
        private IEnemyAgent agent;

        public EnemyType EnemyType => agent.EnemyType;

        public event Action OnDamageEvent;

        public event Action OnTakeDamageEvent;
        
        public event Action<ISwappable> OnAddSwappableItem;

        public void Execute()
        {
            agent = GetComponent<IEnemyAgent>();
            agent.OnGetHealth = () => CurrentHealth;
            agent.OnTakeDamageEvent += () => OnTakeDamageEvent?.Invoke();
            agent.OnAddSwappableItem += OnAddSwappableItem;
            agent.FlowExecute();
        }

        public void OnDamage(uint damage, Vector3 hitPoint, Transform attacker)
        {
            OnDamageEvent?.Invoke();
            CurrentHealth -= damage;
            if (CurrentHealth <= 0)
            {
                OnHealth0Event?.Invoke();
                agent.FlowCancel();
                Destroy(gameObject);
                OnRemoveEvent?.Invoke();
            }
        }

        public void SetHealth(uint health)
        {
            MaxHealth = health;
            CurrentHealth = health;
        }

        public event Action OnHealth0Event;

        public event Action OnRemoveEvent;
        public uint MaxHealth { get; private set; }

        public uint CurrentHealth { get; private set; }

        public bool IsVisible => true;
        
        public GameObject GameObject() => gameObject;
    }
}