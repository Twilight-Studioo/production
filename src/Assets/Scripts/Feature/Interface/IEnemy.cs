#region

using System;
using UnityEngine;

#endregion

namespace Feature.Interface
{
    public interface IEnemy
    {
        public void OnDamage(uint damage, Vector3 hitPoint, Transform attacker);

        public event Action OnDestroyEvent;

        public event Action OnDamageEvent;

        public event Action OnTakeDamageEvent;

        public void Execute();

        public void SetHealth(uint health);
    }
}