using UnityEngine;
using System.Collections;
using Feature.Component;
using Core.Utilities;

public class GunnerController : MonoBehaviour
{
    public EnemyParams enemyParams;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public GameObject bulletRay;

    public Animator Animator { get; private set; }
    public Rigidbody rb { get; private set; }
    private Transform targetPlayer;
    private FSM fsm;
    private int currentAmmo;
    public int attackCount; // 用于记录攻击次数
    public int specialAttackCount;

    private void Start()
    {
        Animator = GetComponent<Animator>();
        fsm = new FSM();
        currentAmmo = enemyParams.MaxAmmo;
        attackCount = 0;
        specialAttackCount = 0;
        rb = GetComponent<Rigidbody>();

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

    public bool IsPlayerOverRange()
    {
        return targetPlayer != null && Vector3.Distance(transform.position, targetPlayer.position) >= enemyParams.OverAttackRange;
    }

    public bool IsPlayerInOverRange()
    {
        return targetPlayer != null &&
               Vector3.Distance(transform.position, targetPlayer.position) <= enemyParams.OverAttackRange &&
               Vector3.Distance(transform.position, targetPlayer.position) > enemyParams.FarDistanceRange;
    }

    public bool IsPlayerOverMidRange()
    {
        return targetPlayer != null &&
               Vector3.Distance(transform.position, targetPlayer.position) <= enemyParams.FarDistanceRange &&
               Vector3.Distance(transform.position, targetPlayer.position) > enemyParams.MidDistanceRange;
    }

    public bool IsPlayerInMidRange()
    {
        return targetPlayer != null &&
               Vector3.Distance(transform.position, targetPlayer.position) <= enemyParams.MidDistanceRange;
    }

    public bool IsPlayerBehind()
    {
        Vector3 directionToPlayer = (targetPlayer.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        return angle > enemyParams.BackAngleThreshold;
    }

    public bool IsCurrentAnimationFinished()
    {
        AnimatorStateInfo stateInfo = Animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.normalizedTime >= 1.0f && !stateInfo.loop;
    }

    public int GetCurrentAttackCount()
    {
        return attackCount;
    }

    public int GetCurrentSpecialAttackCount()
    {
        return specialAttackCount;
    }

    public void FacePlayer()
    {
        if (targetPlayer == null) return;

        Vector3 direction = (targetPlayer.position - transform.position).normalized;
        direction.y = 0;
        transform.forward = direction;
    }
}
