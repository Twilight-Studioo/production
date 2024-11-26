#region

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Feature.Component
{
    public class PlayerHpBar : MonoBehaviour
    {
        private const float DelaySpeed = 1.0f;
        public Image hpBackground;
        public Image hpDelay;
        public Image hpDelay2;
        public Image hpCurrent;
        public TextMeshProUGUI hpText;
        private Coroutine delayCoroutine;

        public void UpdateHealthBar(int currentHealth, int maxHealth)
        {
            var currentHealthRatio = (float)currentHealth / maxHealth;

            hpCurrent.fillAmount = currentHealthRatio;

            if (delayCoroutine != null)
            {
                StopCoroutine(delayCoroutine);
            }

            delayCoroutine = StartCoroutine(UpdateDelayBar(currentHealthRatio));

            if (hpText != null)
            {
                hpText.text = currentHealth + " % ";
            }
        }

        private IEnumerator UpdateDelayBar(float targetFillAmount)
        {
            while (hpDelay.fillAmount > targetFillAmount)
            {
                hpDelay.fillAmount = Mathf.Lerp(hpDelay.fillAmount, targetFillAmount, DelaySpeed * Time.deltaTime);
                yield return null;
            }

            hpDelay.fillAmount = targetFillAmount;
        }
    }
}