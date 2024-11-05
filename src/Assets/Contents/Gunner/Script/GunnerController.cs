using UnityEngine;
using System.Collections;

public class GunnerController : MonoBehaviour
{
    public EnemyParams enemyParams;
    public GameObject bulletPrefab; 
    public Transform bulletSpawnPoint;

    private Animation animationComponent;
    private Transform targetPlayer;
    private Rigidbody rb;
    private int attackCount = 0;
    private bool isAttacking = false;
    private bool isAttackCooldown = false;
    private bool isSpecialAttack = false;
    private float moveTimer = 0f;

    private FSM fsm;

    private void Start()
    {
        animationComponent=GetComponent<Animation>();    
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
            CheckForCQBAttack();
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
        return targetPlayer != null && Vector3.Distance(transform.position, targetPlayer.position) <= enemyParams.DetectionRange;
    }

    public bool IsCloseEnoughToAttack()
    {
        return targetPlayer != null && Vector3.Distance(transform.position, targetPlayer.position) <= enemyParams.AttackRange;
    }

    private bool IsPlayerBehind()
    {
        Vector3 toPlayer = (targetPlayer.position - transform.position).normalized;
        float dotProduct = Vector3.Dot(transform.forward, toPlayer);
        return dotProduct < 0; 
    }

    public void MoveTowardsPlayer()
    {
        if (targetPlayer == null || isAttackCooldown) return;

        if (isSpecialAttack)
        {
            GroundMovement();
        }
        else
        {
            AirJumpMovement();
        }

        FacePlayer();
    }

    private void FacePlayer()
    {
        if (targetPlayer == null) return;
        Vector3 directionToPlayer = (targetPlayer.position - transform.position).normalized;
        directionToPlayer.y = 0;

        transform.rotation = Quaternion.LookRotation(directionToPlayer);
    }

    private void AirJumpMovement()
    {
        Vector3 directionToPlayer = (targetPlayer.position - transform.position).normalized;
        directionToPlayer.y = 0;

        rb.AddForce(new Vector3(0, enemyParams.JumpHeight, 0), ForceMode.Impulse); 
    }

    private void GroundMovement()
    {
        Vector3 directionToPlayer = (targetPlayer.position - transform.position).normalized;
        directionToPlayer.y = 0;

        rb.MovePosition(transform.position + directionToPlayer * enemyParams.MoveSpeed * Time.fixedDeltaTime); // 水平移动
    }

    private IEnumerator MoveThenAttack()
    {
        moveTimer = 0f;

        while (moveTimer < enemyParams.MoveDurationBeforeAttack)
        {
            MoveTowardsPlayer();
            moveTimer += Time.fixedDeltaTime;
            yield return null;
        }

        PerformAttack();
    }

    public void PerformAttack()
    {
        if (isAttackCooldown || isSpecialAttack) return;

        if (IsPlayerBehind())
        {
            PlayAnimation("GUN__attackB"); 
            FacePlayer(); 
            return;
        }

        if (attackCount >= enemyParams.AttacksBeforeSpecialMove)
        {
            StartCoroutine(SpecialMoveCoroutine());
            attackCount = 0;
        }
        else
        {
            string[] attackAnimations = { "GUN__attackFA", "GUN__attackFB", "GUN__attackFC" };
            string selectedAnimation = attackAnimations[Random.Range(0, attackAnimations.Length)];
            PlayAnimation(selectedAnimation);

            FacePlayer(); 
            StartCoroutine(KnockbackCoroutine());
            StartCoroutine(AttackCooldownCoroutine());
            attackCount++;
        }
    }

    public void Shoot()
    {
        Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
    }

    private IEnumerator SpecialMoveCoroutine()
    {
        if (targetPlayer == null) yield break;

        isSpecialAttack = true;

        rb.AddForce(new Vector3(0, enemyParams.JumpHeight, 0), ForceMode.Impulse);
        yield return new WaitForSeconds(0.5f);

        PlayAnimation("GUN__beam");
        yield return new WaitForSeconds(animationComponent["GUN__beam"].length);

        StartCoroutine(KnockbackCoroutine());
        yield return new WaitForSeconds(0.5f);

        rb.AddForce(new Vector3(0, -enemyParams.JumpHeight, 0), ForceMode.Impulse);
        yield return new WaitForSeconds(0.5f);

        isSpecialAttack = false;

        StartCoroutine(AttackCooldownCoroutine());
    }

    private IEnumerator KnockbackCoroutine()
    {
        if (targetPlayer == null) yield break;

        Vector3 directionAwayFromPlayer = (transform.position - targetPlayer.position).normalized;
        rb.AddForce(directionAwayFromPlayer * enemyParams.KnockbackDistance, ForceMode.Impulse);

        yield return new WaitForSeconds(0.5f);

        PlayAnimation("GUN__landing");
    }

    private IEnumerator AttackCooldownCoroutine()
    {
        isAttackCooldown = true;
        yield return new WaitForSeconds(enemyParams.BasicAttackCooldown);
        isAttackCooldown = false;

        StartCoroutine(MoveThenAttack());
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
        Shoot();
        Debug.Log("Performing Attack");
    }

    public void SpecialAttack()
    {
        Debug.Log("Performing Special Attack");
    }
    private void CheckForCQBAttack()
    {
        if (Vector3.Distance(transform.position, targetPlayer.position) <= enemyParams.CQBAttackRange)
        {
            PlayAnimation("GUN__attack_CQB");
        }
    }

    public void StartAttackCooldown(float cooldownTime)
    {
        StartCoroutine(AttackCooldownCoroutine(cooldownTime));
    }

    public bool ShouldPerformSpecialAttack()
    {
        return attackCount >= enemyParams.AttacksBeforeSpecialMove;
    }

    private IEnumerator AttackCooldownCoroutine(float cooldownTime)
    {
        isAttackCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        isAttackCooldown = false;
    }

    public void PlayAnimation(string animationName)
    {
        if (animationComponent != null && animationComponent[animationName] != null)
        {
            animationComponent.CrossFade(animationName);
        }
        else
        {
            Debug.LogWarning($"Animation '{animationName}' not found on {gameObject.name}");
        }
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
