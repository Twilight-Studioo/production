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

        public event DamageHandler<uint, Vector3> OnDamageEvent;

        public event Action OnTakeDamageEvent;

        public void Execute()
        {
            agent = GetComponent<IEnemyAgent>();
            agent.OnGetHealth = () => CurrentHealth;
            agent.RequireDestroy = () => { OnDamage(CurrentHealth, Vector3.zero, null); };
            agent.OnTakeDamageEvent += () => OnTakeDamageEvent?.Invoke();
            agent.FlowExecute();
        }

        public DamageResult OnDamage(uint damage, Vector3 hitPoint, Transform attacker)
        {
            OnDamageEvent?.Invoke(damage, attacker.position);
            CurrentHealth -= damage;
            Debug.Log($"CurrentHealth {CurrentHealth}");
            if (CurrentHealth <= 0)
            {
                // delete 
                OnHealth0Event?.Invoke();
                agent.FlowCancel();
                agent.Delete();
                if (gameObject != null)
                {
                    Destroy(gameObject);
                }
                OnRemoveEvent?.Invoke();
                return new DamageResult.Killed(transform);
            }
            else
            {
                // hit event for agent
                agent.OnDamage(damage, hitPoint, attacker);
                return new DamageResult.Damaged(transform);
            }
        }

        public void SetHealth(uint health)
        {
            MaxHealth = health;
            CurrentHealth = health;
        }

        public event Action OnHealth0Event;

        public GameObject GameObject() => gameObject;

        public event Action OnRemoveEvent;
        public uint MaxHealth { get; private set; }

        public uint CurrentHealth { get; private set; }

        public bool IsVisible => true;
    }
}