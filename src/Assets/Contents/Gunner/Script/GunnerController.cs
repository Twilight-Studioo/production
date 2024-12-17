using Core.Utilities;
using Feature.Component;
using Feature.Interface;
using System;
using System.Collections;
using UnityEngine;

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
    public int attackCount;
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
       // Debug.Log(Vector3.Distance(targetPlayer.position, transform.position));
        if (targetPlayer == null)
        {
            FindPlayer();
        }
        else
        {
            fsm.Update();
        }
    }
    private void ShootBullet()
    {
        var dir = transform.forward.normalized;
        var bullet = ObjectFactory.Instance.CreateObject(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        bullet.transform.position = new Vector3(bullet.transform.position.x, 1.5f, 0);
        var bulletRb = bullet.GetComponent<DamagedTrigger>();
        bulletRb.SetHitObject(false, true, true);
        bulletRb.Execute(dir, enemyParams.BulletSpeed, enemyParams.Damage, enemyParams.BulletLifeTime);
        //bulletRb.OnHitEvent += () => onHitBullet?.Invoke();
    }
    private void ShootFlyBullet()
    {
        var bullet = ObjectFactory.Instance.CreateObject(bulletPrefab, targetPlayer.position + new Vector3(0, 10, 0), Quaternion.identity);
        var dir = (targetPlayer.position - bullet.transform.position).normalized;
        //bullet.transform.position = new Vector3(bullet.transform.position.x, 1.5f, 0);
        var bulletRb = bullet.GetComponent<DamagedTrigger>();
        bulletRb.SetHitObject(false, true, true);
        bulletRb.Execute(dir, enemyParams.BulletSpeed, enemyParams.Damage, enemyParams.BulletLifeTime);
        //bulletRb.OnHitEvent += () => onHitBullet?.Invoke();
    }
    private void ShootRayBullet()
    {
        var bullet = ObjectFactory.Instance.CreateObject(bulletRay, bulletSpawnPoint.position, Quaternion.identity);
        var dir = transform.forward.normalized;
        bullet.transform.position = bulletSpawnPoint.position + new Vector3(dir.x, 0, 0) * (bullet.transform.localScale.x / 2);
        bullet.transform.position = new Vector3(bullet.transform.position.x, bullet.transform.position.y, 0);
        bullet.transform.localScale = new Vector3(bullet.transform.localScale.x, enemyParams.RayW, 1);
        var bulletRb = bullet.GetComponent<DamagedTrigger>();
        bulletRb.SetHitObject(false, true, false);
        bulletRb.Execute(dir, 0, enemyParams.Damage, enemyParams.RayLifeTime);
        //bulletRb.OnHitEvent += () => onHitBullet?.Invoke();
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
        return targetPlayer != null && Vector3.Distance(transform.position, targetPlayer.position) <= enemyParams.OverAttackRange
            && Vector3.Distance(transform.position, targetPlayer.position) > enemyParams.FarDistanceRange;
    }
    public bool IsPlayerOverMidRange()
    {
        return targetPlayer != null && Vector3.Distance(transform.position, targetPlayer.position) <= enemyParams.FarDistanceRange &&
            Vector3.Distance(transform.position, targetPlayer.position) > enemyParams.MidDistanceRange;
    }
    public bool IsPlayerInMidRange()
    {
        return targetPlayer != null && Vector3.Distance(transform.position, targetPlayer.position) <= enemyParams.MidDistanceRange;

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
            int attackType = UnityEngine.Random.Range(0, 2);
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
    public void ApplyForwordMovement()
    {
        //float directionMultiplier = 1;
        //float moveDistance = directionMultiplier * enemyParams.ForwardDistance;
        //Vector3 direction = (targetPlayer.position - transform.position).normalized;
        //direction.y = 0;
        //direction.z = 0;
        //Vector3 moveVector = direction * moveDistance;
        //rb.MovePosition(rb.position + moveVector);

        //Debug.Log($"Rigidbody Movement applied: {moveVector}, New position: {rb.position}");

        Vector3 direction = (targetPlayer.position - transform.position).normalized;
        direction.y = 0; // 保持水平移动
        rb.velocity = direction * enemyParams.ForwardDistance;

        Debug.Log($"Applying forward movement with velocity: {rb.velocity}");
    }
    public void ApplyBackMovement()
    {
        //float directionMultiplier = -1;
        //float moveDistance = directionMultiplier * enemyParams.KnockbackDistance;
        //Vector3 direction = (targetPlayer.position - transform.position).normalized;
        //direction.y = 0;
        //direction.z = 0;
        //Vector3 moveVector = direction * moveDistance;
        //rb.MovePosition(rb.position + moveVector);

        //Debug.Log($"Rigidbody Movement applied: {moveVector}, New position: {rb.position}");

        Vector3 direction = (transform.position - targetPlayer.position).normalized;
        direction.y = 0; // 保持水平移动
        rb.velocity = direction * enemyParams.KnockbackDistance;

        Debug.Log($"Applying backward movement with velocity: {rb.velocity}");
    }
    private GameObject bullet;
    public void ShootFlyRayBullet()
    {
        Debug.Log(22222);
        bullet = ObjectFactory.Instance.CreateObject(bulletRay, bulletSpawnPoint.position, Quaternion.identity);
        bullet.transform.localScale = new Vector3(bullet.transform.localScale.x, enemyParams.RayW, 1);
        bullet.transform.position = new Vector3(bullet.transform.position.x, bullet.transform.position.y, 0);
        var bulletRb = bullet.GetComponent<DamagedTrigger>();
        bulletRb.OnDestroyEvent += BulletRb_OnDestroyEvent;
        bulletRb.SetHitObject(false, true, false);
        bulletRb.Execute(Vector3.down, 0, enemyParams.Damage, 1000);
        StartCoroutine(RotateRay(bullet));
        //
    }

    private void BulletRb_OnDestroyEvent()
    {
        StopAllCoroutines();
        ChangeState(new IdleState(this));
        if (bullet != null)
            Destroy(bullet);
    }

    IEnumerator RotateRay(GameObject bullet)
    {
        while (bullet.transform.eulerAngles.z <= 128)
        {
            bullet.transform.Rotate(Vector3.Lerp(Vector3.zero, new Vector3(0, 0, 130), Time.deltaTime));
            yield return new WaitForSeconds(.3f);
        }
        ChangeState(new IdleState(this));
        if (bullet != null)
            Destroy(bullet);
    }
    public bool IsCurrentAnimationFinished()
    {
        AnimatorStateInfo stateInfo = Animator.GetCurrentAnimatorStateInfo(0);
        //Debug.Log(!stateInfo.loop);
        return stateInfo.normalizedTime >= 1.0f && !stateInfo.loop;
    }
    public bool IsOutOfAmmo()
    {
        return currentAmmo <= 0;
    }

    public void ReloadAmmo()
    {
        currentAmmo = enemyParams.MaxAmmo;
        attackCount = 0;
        specialAttackCount = 0;
        Debug.Log("Ammo reloaded");
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
        direction.z = 0;
        transform.forward = direction;
    }
    public void StopMovement()
    {
        rb.velocity = Vector3.zero;
        Debug.Log("Stopping movement, velocity set to zero.");
    }
}