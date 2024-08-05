#region

using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Core.Utilities.Health
{
    [RequireComponent(typeof(Slider))]
    public class HealthBar : MonoBehaviour
    {
        public Slider healthBar;
        private IHealthBar target;

        private void Update()
        {
            if (target != null)
            {
                var value = (float)target.CurrentHealth / target.MaxHealth;
                healthBar.value = value;
            }
        }

        public void Initialize(IHealthBar healthTarget)
        {
            target = healthTarget;
            healthBar = GetComponent<Slider>();
        }
    }
}