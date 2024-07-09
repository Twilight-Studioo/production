using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public PlayerHPBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.hpCurrent.transform.localScale = new Vector3(3, 0.3f, 1);
            healthBar.hpDelay.transform.localScale = new Vector3(3, 0.3f, 1);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        UpdateHealthBar(); 
        Debug.Log("Player has died.");
        ///gameObject.SetActive(false);
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.UpdateHealthBar();
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}
