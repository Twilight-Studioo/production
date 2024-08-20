#region

using System;
using System.Collections;
using Core.Utilities.Health;
using DynamicActFlow.Runtime.Core.Flow;
using Feature.Interface;
using UnityEngine;

#endregion

namespace Feature.Enemy
{
    public class DroneController : FlowScope, IHealthBar, IEnemy
    {
        [SerializeField] private uint maxHealth = 100;
        private IEnemy enemyImplementation;

        private void Awake()
        {
            autoPlay = false;
            IsVisible = false;
        }

        public void OnDamage(uint damage, Vector3 hitPoint, Transform attacker)
        {
            throw new NotImplementedException();
        }

        public event Action OnHealth0Event;
        public event Action OnDamageEvent;
        public event Action OnTakeDamageEvent;

        public void Execute()
        {
        }

        public void SetHealth(uint health)
        {
            throw new NotImplementedException();
        }

        public uint MaxHealth => maxHealth;
        public uint CurrentHealth { get; private set; }

        public bool IsVisible { get; private set; }
        public event Action OnRemoveEvent;


        public void OnDamage(uint damage)
        {
            CurrentHealth -= damage;
        }

        protected override IEnumerator Flow(IFlowBuilder context)
        {
            IsVisible = true;
            yield return Wait(1.0f);
            IsVisible = false;
        }
    }
}