using UnityEngine;
using System.Collections;

public class GunnerController : MonoBehaviour
{
    public float AttackRange = 10f;
    public float DetectionRange = 20f;
    public int MaxAmmo = 6;
    public float ReloadTime = 2f;
    public float MoveSpeed = 3.5f;
    public float JumpHeight = 8f;
    public float SpecialMoveSpeed = 6f;
    public float KnockbackDistance = 3f;
    public float AttackCooldown = 5f;
    public int AttacksBeforeSpecialMove = 3;
    public float ForwardTeleportDistance = 5f;

    private Transform targetPlayer;
    private Rigidbody rb;
    private int attackCount = 0;
    private bool isAttacking = false;
    private bool isAttackCooldown = false;
    private bool isSpecialAttack = false;

    private FSM fsm;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        fsm = new FSM();
        FindPlayer();
        fsm.SetState(new IdleState(this));
    }

    private void FixedUpdate()
    {
        if (targetPlayer == null)
        {
            FindPlayer();
        }
        else
        {
            fsm.Update();
        }
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            targetPlayer = playerObject.transform;
        }
    }

    public bool IsPlayerInRange()
    {
        return targetPlayer != null && Vector3.Distance(transform.position, targetPlayer.position) <= DetectionRange;
    }

    public bool IsCloseEnoughToAttack()
    {
        return targetPlayer != null && Vector3.Distance(transform.position, targetPlayer.position) <= AttackRange;
    }

    public void MoveTowardsPlayer()
    {
        if (targetPlayer == null || isAttackCooldown) return;

        Vector3 directionToPlayer = (targetPlayer.position - transform.position).normalized;
        directionToPlayer.y = 0;

        rb.MovePosition(transform.position + directionToPlayer * ForwardTeleportDistance);

        FacePlayer(directionToPlayer);
    }

    public void PerformAttack()
    {
        if (isAttackCooldown || isSpecialAttack) return;

        if (attackCount >= AttacksBeforeSpecialMove)
        {
            StartCoroutine(SpecialMoveCoroutine());
            attackCount = 0;
        }
        else
        {
            Attack();
            StartCoroutine(KnockbackCoroutine());
            StartCoroutine(AttackCooldownCoroutine());
            attackCount++;
        }
    }

    private IEnumerator SpecialMoveCoroutine()
    {
        if (targetPlayer == null) yield break;

        isSpecialAttack = true;

        rb.velocity = new Vector3(rb.velocity.x, JumpHeight, rb.velocity.z);

        yield return new WaitForSeconds(0.5f);

        Vector3 targetPosition = new Vector3(targetPlayer.position.x, transform.position.y, transform.position.z);

        rb.MovePosition(new Vector3(targetPosition.x, transform.position.y, transform.position.z));

        yield return new WaitForSeconds(0.5f);

        SpecialAttack();

        StartCoroutine(KnockbackCoroutine());

        yield return new WaitForSeconds(0.5f);

        rb.velocity = new Vector3(rb.velocity.x, -JumpHeight, rb.velocity.z);

        yield return new WaitForSeconds(0.5f);

        isSpecialAttack = false;

        StartCoroutine(AttackCooldownCoroutine());
    }

    private IEnumerator KnockbackCoroutine()
    {
        if (targetPlayer == null) yield break;

        Vector3 directionAwayFromPlayer = (transform.position - targetPlayer.position).normalized;

        rb.AddForce(directionAwayFromPlayer * KnockbackDistance, ForceMode.Impulse);

        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator AttackCooldownCoroutine()
    {
        isAttackCooldown = true;
        yield return new WaitForSeconds(AttackCooldown);
        isAttackCooldown = false;
    }

    private void FacePlayer(Vector3 directionToPlayer)
    {
        if (targetPlayer == null) return;

        Vector3 direction = (targetPlayer.position - transform.position).normalized;
        direction.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = targetRotation;
    }

    public void Attack()
    {
        Debug.Log("Performing Attack");
    }

    public void SpecialAttack()
    {
        Debug.Log("Performing Special Attack");
    }

    public bool IsOutOfAmmo()
    {
        return false;
    }

    public void ReloadAmmo()
    {
        Debug.Log("Reloading Ammo");
    }

    public void ChangeState(IState newState)
    {
        fsm.SetState(newState);
    }
}
