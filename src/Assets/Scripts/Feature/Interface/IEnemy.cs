#region

using System;
using Feature.Common.Constants;
using UnityEngine;

#endregion

namespace Feature.Interface
{
    public interface IEnemy
    {
        public EnemyType EnemyType { get; }
        public DamageResult OnDamage(uint damage, Vector3 hitPoint, Transform attacker);

        public event Action OnHealth0Event;

        public event DamageHandler<uint, Vector3> OnDamageEvent;

        public event Action OnTakeDamageEvent;

        public void Execute();

        public void SetHealth(uint health);

        public GameObject GameObject();
    }
}