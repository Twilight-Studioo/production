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

        public event DamageHandler<DamageResult, Vector3> OnDamageEvent;

        public event Action OnTakeDamageEvent;

        public void Execute()
        {
            agent = GetComponent<IEnemyAgent>();
            agent.OnGetHealth = () => CurrentHealth;
            agent.RequireDestroy = () => { OnDamage((uint)CurrentHealth, Vector3.zero, null); };
            agent.OnTakeDamageEvent += () => OnTakeDamageEvent?.Invoke();
            agent.FlowExecute();
        }

        public DamageResult OnDamage(uint damage, Vector3 hitPoint, Transform attacker)
        {
            if (CurrentHealth <= 0)
            {
                return new DamageResult.Missed();
            }
            if (CurrentHealth - damage <= 0)
            {
                CurrentHealth = 0;
                OnDamageEvent?.Invoke(new DamageResult.Killed(transform), hitPoint);
                // delete 
                OnHealth0Event?.Invoke();
                agent.FlowCancel();
                agent.DestroyEnemy();
                agent.Delete();
                if (gameObject != null)
                {
                    Destroy(gameObject, 3f);
                }
                OnRemoveEvent?.Invoke();
                return new DamageResult.Killed(transform);
            }
            else
            {
                CurrentHealth -= (int)damage;
                OnDamageEvent?.Invoke(new DamageResult.Damaged(transform), hitPoint);
                // hit event for agent
                agent.OnDamage(damage, hitPoint, attacker);
                return new DamageResult.Damaged(transform);
            }
        }

        public void SetHealth(uint health)
        {
            MaxHealth = (int)health;
            CurrentHealth = (int)health;
        }

        public event Action OnHealth0Event;

        public GameObject GameObject() => gameObject;

        public event Action OnRemoveEvent;
        public int MaxHealth { get; private set; }

        public int CurrentHealth { get; private set; }

        public bool IsVisible => true;
    }
}