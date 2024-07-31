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

        private void Awake()
        {
            autoPlay = false;
            IsVisible = false;
        }


        public void OnDamage(uint damage)
        {
            CurrentHealth -= damage;
        }

        public event Action OnDestroyEvent;

        public void Execute()
        {
        }

        public uint MaxHealth => maxHealth;
        public uint CurrentHealth { get; private set; }

        public bool IsVisible { get; private set; }

        protected override IEnumerator Flow(IFlowBuilder context)
        {
            IsVisible = true;
            yield return Wait(1.0f);
            IsVisible = false;
        }
    }
}