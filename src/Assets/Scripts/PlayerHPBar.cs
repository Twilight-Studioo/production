using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPBar : MonoBehaviour
{
    public Image hpBackground;
    public Image hpDelay;
    public Image hpCurrent;
    public PlayerHP playerHealth;

    private float delaySpeed = 1.0f;

    void Start()
    {
        if (playerHealth == null)
        {
            playerHealth = GetComponent<PlayerHP>();
        }
    }

    void Update()
    {
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        float currentHealthRatio = (float)playerHealth.GetCurrentHealth() / playerHealth.maxHealth;
        hpCurrent.transform.localScale = new Vector3(currentHealthRatio * 3, 0.3f, 1);

        if (hpDelay.transform.localScale.x > hpCurrent.transform.localScale.x)
        {
            hpDelay.transform.localScale = new Vector3(Mathf.Lerp(hpDelay.transform.localScale.x, hpCurrent.transform.localScale.x, delaySpeed * Time.deltaTime), 0.3f, 1);
        }
    }
}
