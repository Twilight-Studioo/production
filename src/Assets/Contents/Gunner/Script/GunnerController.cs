using UnityEngine;
using System.Collections;

public class GunnerController : MonoBehaviour
{
    public EnemyParams enemyParams;
    public GameObject bulletPrefab; 
    public Transform bulletSpawnPoint; 

    public Animator Animator { get; private set; }
    private Transform targetPlayer;
    private Rigidbody rb;
    private FSM fsm;
    private int currentAmmo;
    private int attackCount;
    private bool isJumping;

    private void Start()
    {
        Animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        fsm = new FSM();
        currentAmmo = enemyParams.MaxAmmo;
        attackCount = 0;
        isJumping = false;

        FindPlayer();

        fsm.SetState(new IdleState(this));
    }

    private void Update()
    {
        if (targetPlayer == null)
        {
            FindPlayer();
        }
        else
        {
            fsm.Update();
        }

        if (isJumping && IsGrounded())
        {
            isJumping = false;
            ChangeState(new LandingState(this));
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

    public void ChangeState(IState newState)
    {
        fsm.SetState(newState);
    }

    public bool IsPlayerInRange()
    {
        return targetPlayer != null && Vector3.Distance(transform.position, targetPlayer.position) <= enemyParams.DetectionRange;
    }
    public bool IsPlayerBehind()
    {
        Vector3 directionToPlayer = (targetPlayer.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        return angle > enemyParams.BackAngleThreshold;
    }

    public bool IsCloseEnoughToAttack()
    {
        return targetPlayer != null && Vector3.Distance(transform.position, targetPlayer.position) <= enemyParams.AttackRange;
    }

    public bool IsOutOfAmmo()
    {
        return currentAmmo <= 0;
    }

    public bool IsGrounded()
    {
        RaycastHit hit;
        float groundCheckDistance = 0.2f; 
        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance))
        {
            return hit.collider.CompareTag("Ground");
        }
        return false;
    }
    public bool IsCloseEnoughToCQB()
    {
        return targetPlayer != null && Vector3.Distance(transform.position, targetPlayer.position) <= enemyParams.CQBAttackRange;
    }

    public bool ShouldPerformSpecialAttack()
    {
        return attackCount >= enemyParams.AttacksBeforeSpecialMove;
    }

    public void ReloadAmmo()
    {
        currentAmmo = enemyParams.MaxAmmo;
        attackCount = 0; 
        Debug.Log("Ammo reloaded");
    }

    public void MoveTowardsPlayer()
    {
        if (targetPlayer == null) return;

        FacePlayer();

        Vector3 direction = (targetPlayer.position - transform.position).normalized;
        rb.MovePosition(transform.position + direction * enemyParams.MoveSpeed * Time.deltaTime);
    }

    private void FacePlayer()
    {
        if (targetPlayer == null) return;

        Vector3 direction = (targetPlayer.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
    }

    private void FacePlayer()
    {
        if (targetPlayer == null) return;

        Vector3 direction = (targetPlayer.position - transform.position).normalized;
        direction.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
    }

    public void PerformAttack()
    {
        if (!IsOutOfAmmo())
        {
            Animator.SetTrigger("Attack");
            currentAmmo--;
            attackCount++;
        }
    }

    public void TriggerJump()
    {
        isJumping = true;

        rb.AddForce(Vector3.up * enemyParams.JumpHeight, ForceMode.Impulse);

        Debug.Log("Jump started");
    }

    public void StartAttackCooldown(float cooldownTime)
    {
        StartCoroutine(AttackCooldownCoroutine(cooldownTime));
    }

    private IEnumerator AttackCooldownCoroutine(float cooldownTime)
    {
        yield return new WaitForSeconds(cooldownTime);
        Debug.Log("Cooldown completed");
    }
}
