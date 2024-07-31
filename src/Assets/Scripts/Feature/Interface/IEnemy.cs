#region

using System;

#endregion

namespace Feature.Interface
{
    public interface IEnemy
    {
        public void OnDamage(uint damage);

        public event Action OnDestroyEvent;

        public void Execute();
    }
}