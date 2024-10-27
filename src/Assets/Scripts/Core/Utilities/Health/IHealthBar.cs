#region

using System;

#endregion

namespace Core.Utilities.Health
{
    public interface IHealthBar
    {
        public uint MaxHealth { get; }

        public uint CurrentHealth { get; }

        public bool IsVisible { get; }

        public event Action OnRemoveEvent;
    }
}