using UnityEngine;

public class PlayerView : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerHP playerHP;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerHP = GetComponent<PlayerHP>();
    }

    public void Move(Vector3 movement)
    {
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
    }

    public void Jump(float jumpForce)
    {
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
    }

    public int GetCurrentHealth()
    {
        return playerHP.GetCurrentHealth();
    }

    public void TakeDamage(int damage)
    {
        playerHP.TakeDamage(damage);
    }
}
