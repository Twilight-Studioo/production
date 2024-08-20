#region

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Feature.Component
{
    public class PlayerHPBar : MonoBehaviour
    {
        public Image hpBackground;
        public Image hpDelay;
        public Image hpCurrent;
        public TextMeshProUGUI hpText;
        private Coroutine delayCoroutine;

        private readonly float delaySpeed = 1.0f;

        public void UpdateHealthBar(int currentHealth, int maxHealth)
        {
            var currentHealthRatio = (float)currentHealth / maxHealth;
            var newWidth = currentHealthRatio * hpBackground.rectTransform.sizeDelta.x;

            hpCurrent.rectTransform.sizeDelta = new(newWidth, hpCurrent.rectTransform.sizeDelta.y);

            if (delayCoroutine != null)
            {
                StopCoroutine(delayCoroutine);
            }

            delayCoroutine = StartCoroutine(UpdateDelayBar(newWidth));

            if (hpText != null)
            {
                hpText.text = currentHealth + " / " + maxHealth;
            }
        }

        private IEnumerator UpdateDelayBar(float targetWidth)
        {
            while (hpDelay.rectTransform.sizeDelta.x > targetWidth)
            {
                hpDelay.rectTransform.sizeDelta =
                    new(Mathf.Lerp(hpDelay.rectTransform.sizeDelta.x, targetWidth, delaySpeed * Time.deltaTime),
                        hpDelay.rectTransform.sizeDelta.y);
                yield return null;
            }

            hpDelay.rectTransform.sizeDelta = new(targetWidth, hpDelay.rectTransform.sizeDelta.y);
        }
    }
}