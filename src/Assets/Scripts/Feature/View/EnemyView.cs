#region

using System;
using Core.Utilities.Health;
using Feature.Enemy;
using Feature.Interface;
using UnityEngine;

#endregion

namespace Feature.View
{
    public class EnemyView : MonoBehaviour, IHealthBar, IEnemy
    {
        private IEnemyAgent agent;

        private void Start()
        {
            agent = GetComponent<IEnemyAgent>();
            agent.FlowExecute();
        }

        public void OnDamage(uint damage)
        {
            throw new NotImplementedException();
        }

        public event Action OnDestroyEvent;
        public uint MaxHealth { get; }
        public uint CurrentHealth { get; }

        public bool IsVisible => true;
    }
}