using UnityEngine;
using System.Collections;

public class GunnerController : MonoBehaviour
{
    public float AttackRange = 10f; 
    public float DetectionRange = 20f;  
    public int MaxAmmo = 6; 
    public float ReloadTime = 2f;  
    //public float MoveSpeed = 3.5f; 
    public float JumpHeight = 5f;
    public float DesiredDistanceFromPlayer = 5f;

    [HideInInspector]
    public FSM fsm;

    private int ammoCount;
    private Transform targetPlayer;
    private Rigidbody rb;

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
        if (targetPlayer == null) return false;
        return Vector3.Distance(transform.position, targetPlayer.position) <= DetectionRange;
    }

    public bool IsCloseEnoughToAttack()
    {
        if (targetPlayer == null) return false;
        return Vector3.Distance(transform.position, targetPlayer.position) <= AttackRange;
    }

    public bool IsPlayerInFront()
    {
        if (targetPlayer == null) return false;
        Vector3 directionToPlayer = targetPlayer.position - transform.position;
        return Vector3.Dot(transform.forward, directionToPlayer.normalized) > 0;
    }

    public bool IsPlayerInAir()
    {
        if (targetPlayer == null) return false;
        return targetPlayer.position.y > 1f; 
    }

    public void MoveTowardsPlayer()
    {
        if (targetPlayer == null) return;

        Vector3 directionToPlayer = (targetPlayer.position - transform.position).normalized;

        Vector3 targetPosition = targetPlayer.position - directionToPlayer * DesiredDistanceFromPlayer;

        transform.position = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);

        FacePlayer();
    }
    private void FacePlayer()
    {
        if (targetPlayer == null) return;

        Vector3 directionToPlayer = (targetPlayer.position - transform.position).normalized;
        directionToPlayer.y = 0;  

        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

        transform.rotation = targetRotation;
    }

    public void JumpAndMoveTowardsPlayer()
    {
        StartCoroutine(JumpCoroutine());
    }

    private IEnumerator JumpCoroutine()
    {
        if (targetPlayer == null) yield break;

        rb.velocity = new Vector3(rb.velocity.x, JumpHeight, rb.velocity.z);
        yield return new WaitForSeconds(1f);  

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
