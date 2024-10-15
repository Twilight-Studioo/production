using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class GunnerController : MonoBehaviour
{
    public Transform Player;  
    public float AttackRange = 10f; 
    public float DetectionRange = 20f; 
    public int MaxAmmo = 6;  
    public float ReloadTime = 2f;  
    public float MoveSpeed = 3.5f; 
    public float JumpHeight = 5f; 

    [HideInInspector]
    public FSM fsm;  

    private int ammoCount;
    private NavMeshAgent agent;

    private void Start()
    {
        fsm = GetComponent<FSM>();  
        agent = GetComponent<NavMeshAgent>();  
        ammoCount = MaxAmmo;  
        fsm.SetState(new IdleState(this));  
    }

    public bool IsPlayerInRange()
    {
        return Vector3.Distance(transform.position, Player.position) <= DetectionRange;
    }

    public bool IsCloseEnoughToAttack()
    {
        return Vector3.Distance(transform.position, Player.position) <= AttackRange;
    }

    public bool IsPlayerInFront()
    {
        Vector3 directionToPlayer = Player.position - transform.position;
        return Vector3.Dot(transform.forward, directionToPlayer.normalized) > 0;
    }

    public bool IsPlayerInAir()
    {
        return Player.position.y > 1f;
    }

    public void MoveTowardsPlayer()
    {
        if (IsPlayerInFront())
        {
            agent.SetDestination(Player.position - Player.forward * 2f); 
        }
        else
        {
            agent.SetDestination(Player.position + Player.forward * 2f); 
        }
    }

    public void JumpAndMoveTowardsPlayer()
    {
        StartCoroutine(JumpCoroutine());
    }

    private IEnumerator JumpCoroutine()
    {
        agent.isStopped = true;
        transform.position += Vector3.up * JumpHeight; 
        yield return new WaitForSeconds(1f);  
        agent.isStopped = false;
        MoveTowardsPlayer();  
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
