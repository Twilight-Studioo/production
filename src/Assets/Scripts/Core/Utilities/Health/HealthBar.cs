#region

using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Core.Utilities.Health
{
    public class HealthBar : MonoBehaviour
    {
        public Image healthBarFill;
        private IHealthBar target;

        private void Update()
        {
            if (target != null)
            {
                healthBarFill.fillAmount = target.CurrentHealth / target.MaxHealth;
            }
        }

        public void Initialize(IHealthBar healthTarget)
        {
            target = healthTarget;
        }
    }
}