using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float detectionRange = 10f;
    public LayerMask playerLayer;
    private Transform player;
    private PlayerHP playerHP;
    private bool isPlayerInRange;

    void Update()
    {
        FindPlayer();
        if (isPlayerInRange)
        {
            ChasePlayer();
        }
    }

    void FindPlayer()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange, playerLayer);
        if (hitColliders.Length > 0)
        {
            player = hitColliders[0].transform;
            playerHP = player.GetComponent<PlayerHP>();
            isPlayerInRange = true;
        }
        else
        {
            isPlayerInRange = false;
        }
    }

    void ChasePlayer()
    {
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerHP.TakeDamage(10);
        }
    }
}
