using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Feature.Component
{
    public class PlayerHPBar : MonoBehaviour
    {
        public Image hpBackground;
        public Image hpDelay;
        public Image hpCurrent;
        public TextMeshProUGUI hpText;

        private float delaySpeed = 1.0f;
        private Coroutine delayCoroutine;

        public void UpdateHealthBar(int currentHealth, int maxHealth)
        {
            float currentHealthRatio = (float)currentHealth / maxHealth;
            float newWidth = currentHealthRatio * hpBackground.rectTransform.sizeDelta.x;

            hpCurrent.rectTransform.sizeDelta = new Vector2(newWidth, hpCurrent.rectTransform.sizeDelta.y);

            if (delayCoroutine != null)
            {
                StopCoroutine(delayCoroutine);
            }
            delayCoroutine = StartCoroutine(UpdateDelayBar(newWidth));

            if (hpText != null)
            {
                hpText.text = currentHealth.ToString() + " / " + maxHealth.ToString();
            }
        }

        private IEnumerator UpdateDelayBar(float targetWidth)
        {
            while (hpDelay.rectTransform.sizeDelta.x > targetWidth)
            {
                hpDelay.rectTransform.sizeDelta = new Vector2(Mathf.Lerp(hpDelay.rectTransform.sizeDelta.x, targetWidth, delaySpeed * Time.deltaTime), hpDelay.rectTransform.sizeDelta.y);
                yield return null;
            }
            hpDelay.rectTransform.sizeDelta = new Vector2(targetWidth, hpDelay.rectTransform.sizeDelta.y);
        }
    }

}
