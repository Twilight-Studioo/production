using UnityEngine;

public class GunnerController : MonoBehaviour
{
    public EnemyParams enemyParams;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;

    public Animator Animator { get; private set; }
    public Rigidbody rb { get; private set; }
    private Transform targetPlayer;
    private FSM fsm;
    private int currentAmmo;
    private int attackCount;

    private void Start()
    {
        Animator = GetComponent<Animator>();
        fsm = new FSM();
        currentAmmo = enemyParams.MaxAmmo;
        attackCount = 0;

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

    public bool IsCloseEnoughToAttack()
    {
        return targetPlayer != null && Vector3.Distance(transform.position, targetPlayer.position) <= enemyParams.AttackRange;
    }

    public bool IsPlayerBehind()
    {
        Vector3 directionToPlayer = (targetPlayer.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        return angle > enemyParams.BackAngleThreshold;
    }

    public void PerformAttack()
    {
        if (!IsOutOfAmmo())
        {
            int attackType = Random.Range(0, 2); 
            if (IsPlayerBehind())
            {
                Animator.SetTrigger("BackAttack");
            }
            else if (IsCloseEnoughToAttack())
            {
                Animator.SetTrigger("CloseAttack");
            }
            else
            {
                if (attackType == 0)
                {
                    Animator.SetTrigger("AttackA");
                }
                else if (attackType == 1)
                {
                    Animator.SetTrigger("AttackB");
                }
            }
            currentAmmo--;
            attackCount++;
        }
    }

    public bool IsOutOfAmmo()
    {
        return currentAmmo <= 0;
    }

    public void ReloadAmmo()
    {
        currentAmmo = enemyParams.MaxAmmo;
        attackCount = 0;
        Debug.Log("Ammo reloaded");
    }

    public void FacePlayer()
    {
        if (targetPlayer == null) return;

        Vector3 direction = (targetPlayer.position - transform.position).normalized;
        direction.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
    }
}
