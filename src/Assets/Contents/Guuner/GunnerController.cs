using UnityEngine;
using System.Collections;

public class GunnerController : MonoBehaviour
{
    public float AttackRange = 10f;
    public float DetectionRange = 20f;
    public int MaxAmmo = 6;
    public float ReloadTime = 2f;
    public float MoveSpeed = 3.5f;
    public float JumpHeight = 5f;
    public float SpecialMoveSpeed = 6f;
    public int MovesBeforeSpecialMove = 3;
    public float KnockbackDistance = 3f;
    public float AttackCooldown = 2f;

    [HideInInspector]
    public FSM fsm;

    private int ammoCount;
    private Transform targetPlayer;
    private Rigidbody rb;
    private int moveCount = 0;

    private void Start()
    {
        fsm = GetComponent<FSM>();
        rb = GetComponent<Rigidbody>();
        ammoCount = MaxAmmo;
        fsm.SetState(new IdleState(this));
        FindPlayer();
    }

    private void Update()
    {
        if (targetPlayer == null)
        {
            FindPlayer();
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
        if (targetPlayer == null) return;

        Vector3 directionToPlayer = (targetPlayer.position - transform.position).normalized;
        directionToPlayer.y = 0;
        rb.MovePosition(transform.position + directionToPlayer * MoveSpeed * Time.deltaTime);
        FacePlayer(directionToPlayer);
        moveCount++;
        if (moveCount >= MovesBeforeSpecialMove || MovesBeforeSpecialMove == 0)
        {
            SpecialMove();
            moveCount = 0;
        }
    }

    public void SpecialMove()
    {
        if (ammoCount > 0)
        {
            StartCoroutine(SpecialMoveCoroutine());
        }
    }

    private IEnumerator SpecialMoveCoroutine()
    {
        if (targetPlayer == null) yield break;

        ammoCount--;
        Vector3 directionToPlayer = (targetPlayer.position - transform.position).normalized;
        Vector3 targetPosition = targetPlayer.position + Vector3.up * JumpHeight;
        targetPosition.z = transform.position.z;

        float startTime = Time.time;
        float duration = 1.0f;
        while (Time.time < startTime + duration)
        {
            rb.MovePosition(Vector3.MoveTowards(transform.position, targetPosition, SpecialMoveSpeed * Time.deltaTime));
            yield return null;
        }
        PerformAttack();
    }

    public void PerformAttack()
    {
        if (IsCloseEnoughToAttack())
        {
            Attack();
            StartCoroutine(KnockbackCoroutine()); 
            StartCoroutine(AttackCooldownCoroutine());
        }
    }

    private IEnumerator KnockbackCoroutine()
    {
        if (targetPlayer == null) yield break;

        // 计算后退方向：从玩家位置到BOSS位置的反方向
        Vector3 directionAwayFromPlayer = (transform.position - targetPlayer.position).normalized;

        // 目标位置是当前BOSS位置加上反方向的距离
        Vector3 knockbackPosition = transform.position + directionAwayFromPlayer * KnockbackDistance;

        float startTime = Time.time;
        float duration = 0.5f;  // 后退的持续时间

        while (Time.time < startTime + duration)
        {
            // 平滑移动BOSS到目标位置
            rb.MovePosition(Vector3.MoveTowards(transform.position, knockbackPosition, MoveSpeed * Time.deltaTime));
            yield return null;
        }
    }

    private IEnumerator AttackCooldownCoroutine()
    {
        yield return new WaitForSeconds(AttackCooldown);
        MoveTowardsPlayer();
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
        ammoCount--;
    }

    public void SpecialAttack()
    {
        Debug.Log("Performing Special Attack");
    }

    public bool IsOutOfAmmo()
    {
        return ammoCount <= 0;
    }

    public void ReloadAmmo()
    {
        ammoCount = MaxAmmo;
    }

    public void ChangeState(IState newState)
    {
        fsm.SetState(newState);
    }
}
