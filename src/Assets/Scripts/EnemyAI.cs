using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float detectionRange = 10f;
    public LayerMask playerLayer;
    private Transform player;
    private PlayerHP playerHealth;
    private bool isPlayerInRange;
    private bool isAttacking;

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
            playerHealth = player.GetComponent<PlayerHP>();
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
            direction.y = 0;
            direction.z = 0;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isAttacking)
        {
            StartCoroutine(AttackPlayer());
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StopCoroutine(AttackPlayer());
            isAttacking = false;
        }
    }

    IEnumerator AttackPlayer()
    {
        isAttacking = true;
        while (isAttacking)
        {
            playerHealth.TakeDamage(10);
            yield return new WaitForSeconds(2f);
        }
    }
}
