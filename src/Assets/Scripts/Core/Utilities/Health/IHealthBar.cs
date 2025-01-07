#region

using System;

#endregion

namespace Core.Utilities.Health
{
    public interface IHealthBar
    {
        public int MaxHealth { get; }

        public int CurrentHealth { get; }

        public bool IsVisible { get; }

        public event Action OnRemoveEvent;
    }
}