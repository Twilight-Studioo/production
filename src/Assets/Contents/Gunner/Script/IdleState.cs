using UnityEngine;
using System.Collections;
using Feature.Component;
using Core.Utilities;
public class IdleState : IState//Gunner默认状态
{
    private GunnerController _gunnerController;

    private float coldTimer;

    public IdleState(GunnerController gunnerController)
    {
        _gunnerController = gunnerController;
    }

    public void Enter()
    {
        _gunnerController.Animator.SetBool("Idle",true);
        coldTimer = Time.time;
    }

    public void Execute()
    {
        GunnerThink();
    }

    public void Exit() { }
    private void GunnerThink()
    {
        if (_gunnerController.IsPlayerBehind())//首先进行转向判定
        {
            _gunnerController.ChangeState(new BackAttackState(_gunnerController));
            _gunnerController.attackCount++;
        }
        if (_gunnerController.GetCurrentAttackCount() >= _gunnerController.enemyParams.SpecialAttackAmmo )//进行特殊攻击判定
        {
            _gunnerController.ChangeState(new SpecialAttackState(_gunnerController));
            _gunnerController.attackCount = 0;
            _gunnerController.specialAttackCount++;
            return;
        }
        if (_gunnerController.GetCurrentSpecialAttackCount() >= _gunnerController.enemyParams.FlyAttackAmmo)//进行飞天攻击判定
        {
            _gunnerController.ChangeState(new FlyAttackState(_gunnerController));
            _gunnerController.specialAttackCount = 0;
            return;
        }
        if (Time.time - coldTimer >= _gunnerController.enemyParams.BasicAttackCooldown)//常规攻击
        {
            if (_gunnerController.IsPlayerOverRange())
            {
                _gunnerController.ChangeState(new OverDistanceAttackState(_gunnerController));
                _gunnerController.attackCount++;
            }
            else if (_gunnerController.IsPlayerInOverRange())
            {
                _gunnerController.ChangeState(new FarDistanceAttackState(_gunnerController));
                _gunnerController.attackCount++;
            }
            else if (_gunnerController.IsPlayerOverMidRange())
            {
                _gunnerController.ChangeState(new MidDistanceAttackState(_gunnerController));
                _gunnerController.attackCount++;
            }
            else if (_gunnerController.IsPlayerInMidRange())
            {
                _gunnerController.ChangeState(new FaceAttackState(_gunnerController));
                _gunnerController.attackCount++;
            }
        }

    }
}
public class FaceAttackState : IState//主角与gunner距离小于18进入此状态
{
    private GunnerController _gunnerController;

    public FaceAttackState(GunnerController gunnerController)
    {
        _gunnerController = gunnerController;
    }

    public void Enter()
    {
        ExecuteCloseAttack();
    }

    public void Execute()
    {

        if (_gunnerController.IsCurrentAnimationFinished())
        {
            //Debug.Log(3333);
           
            _gunnerController.ChangeState(new IdleState(_gunnerController));
        }
    }



    public void Exit()
    {
        _gunnerController.Animator.SetBool("CloseAttack", false);
    }


    private void ExecuteCloseAttack()
    {
        _gunnerController.Animator.SetBool("CloseAttack",true);
       //_gunnerController.ApplyMovement(true);
        Debug.Log("Executing CloseAttack with forward movement");
    }
}
public class MidDistanceAttackState : IState//距离在18到30之间进入此攻击状态
{
    private GunnerController _gunnerController;

    public MidDistanceAttackState(GunnerController gunnerController)
    {
        _gunnerController = gunnerController;
    }
    public void Enter()
    {
        //Debug.Log("MidDistanceAttackState");
        ExecuteRangedAttack();
    }

    public void Execute()
    {
        if (_gunnerController.IsCurrentAnimationFinished())
        {
            _gunnerController.ChangeState(new IdleState(_gunnerController));
        }
    }

    public void Exit()
    {
        _gunnerController.Animator.SetBool("AttackA", false);
        _gunnerController.Animator.SetBool("AttackB", false);
    }
    private void ExecuteRangedAttack()
    {
        int attackType = Random.Range(0, 2);
        if (attackType == 0)
        {
            _gunnerController.Animator.SetBool("AttackA", true);
           // _gunnerController.ApplyMovement(false);
            Debug.Log("Executing AttackA with backward movement");
        }
        else
        {
            _gunnerController.Animator.SetBool("AttackB", true);
            //_gunnerController.ApplyMovement(false);
            Debug.Log("Executing AttackB with backward movement");
        }
    }
}
public class FarDistanceAttackState : IState//距离在30到50之间进入此攻击状态
{
    private GunnerController _gunnerController;

    public FarDistanceAttackState(GunnerController gunnerController)
    {
        _gunnerController = gunnerController;
    }
    public void Enter()
    {
        _gunnerController.Animator.SetBool("AttackFC", true);

    }

    public void Execute()
    {
       if(_gunnerController.IsCurrentAnimationFinished())
        {
            _gunnerController.ChangeState(new IdleState(_gunnerController));
        }
    }

    public void Exit()
    {
        _gunnerController.Animator.SetBool("AttackFC", false);
    }
}
public class OverDistanceAttackState : IState//距离在50以上进入此攻击状态
{
    private GunnerController _gunnerController;

    public OverDistanceAttackState(GunnerController gunnerController)
    {
        _gunnerController = gunnerController;
    }
    public void Enter()
    {
        _gunnerController.Animator.SetBool("FarAttack", true);
    }

    public void Execute()
    {
        if(_gunnerController.IsCurrentAnimationFinished())
        {
            _gunnerController.ChangeState(new FarDistanceAttackState(_gunnerController));
        }
    }

    public void Exit()
    {
        _gunnerController.Animator.SetBool("FarAttack", false);
    }
}

public class SpecialAttackState : IState//特殊攻击状态
{
    private GunnerController _gunnerController;

    public SpecialAttackState(GunnerController gunnerController)
    {
        _gunnerController = gunnerController;
    }
    public void Enter()
    {
        _gunnerController.Animator.SetBool("SpecialAttack", true);
    }

    public void Execute()
    {
       if(_gunnerController.IsCurrentAnimationFinished())
        {
            _gunnerController.ChangeState(new IdleState(_gunnerController));
        }
    }

    public void Exit()
    {
        _gunnerController.Animator.SetBool("SpecialAttack", false);
    }
}

public class BackAttackState : IState//背后攻击状态
{
    private GunnerController _gunnerController;

    public BackAttackState(GunnerController gunnerController)
    {
        _gunnerController = gunnerController;
    }
    public void Enter()
    {
        //Debug.Log("BackAttackState");
        ExecuteBackAttack();
    }

    public void Execute()
    {
        if(IsCurrentAnimationFinished())
        {
            _gunnerController.ChangeState(new IdleState(_gunnerController));
        }
    }

    public void Exit()
    {
        _gunnerController.Animator.SetBool("Back", false);
    }
    private bool IsCurrentAnimationFinished()
    {
        AnimatorStateInfo stateInfo = _gunnerController.Animator.GetCurrentAnimatorStateInfo(0);
        //Debug.Log(!stateInfo.loop);
        return stateInfo.normalizedTime >= 1.0f; //&& !stateInfo.loop;
    }
    private void ExecuteBackAttack()
    {
        //_gunnerController.FacePlayer();
        _gunnerController.Animator.SetBool("Back", true);
        ApplyMovement(false);
        Debug.Log("Executing BackAttack with forward movement");
    }
    private void ApplyMovement(bool forward)
    {
        float directionMultiplier = forward ? 1 : -1;
        float moveDistance = directionMultiplier * _gunnerController.enemyParams.KnockbackDistance;

        Vector3 moveVector = _gunnerController.transform.forward * moveDistance;
        _gunnerController.rb.MovePosition(_gunnerController.rb.position + moveVector);

        Debug.Log($"Rigidbody Movement applied: {moveVector}, New position: {_gunnerController.rb.position}");
    }
}

public class FlyAttackState : IState//特殊攻击触发的飞天攻击
{
    private GunnerController _gunnerController;

    public FlyAttackState(GunnerController gunnerController)
    {
        _gunnerController = gunnerController;
    }
    public void Enter()
    {
        _gunnerController.transform.position += new Vector3(0, 10, 0);
        _gunnerController.rb.isKinematic = true;
       _gunnerController.ShootFlyRayBullet();
    }

    public void Execute()
    {
       
    }
   
    public void Exit()
    {
        //_gunnerController.transform.position -= new Vector3(0, 10, 0);
        _gunnerController.rb.isKinematic = false;
        _gunnerController.Animator.SetTrigger("Landing");
    }
}
