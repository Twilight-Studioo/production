using UnityEngine;
using System.Collections;
using System.Xml;
public class IdleState : IState
{
    private GunnerController _gunnerController;

    public IdleState(GunnerController gunnerController)
    {
        _gunnerController = gunnerController;
    }

    public void Enter()
    {
        _gunnerController.Animator.SetTrigger("Idle");
    }

    public void Execute()
    {
        if (_gunnerController.IsPlayerInRange())
        {
            _gunnerController.ChangeState(new MoveState(_gunnerController));
        }
    }

    public void Exit() { }
}
public class MoveState : IState
{
    private GunnerController _gunnerController;

    public MoveState(GunnerController gunnerController)
    {
        _gunnerController = gunnerController;
    }

    public void Enter()
    {
        Debug.Log("Entering Move State");
    }

    public void Execute()
    {
        _gunnerController.MoveTowardsPlayer();

        if (_gunnerController.IsCloseEnoughToAttack())
        {
            _gunnerController.ChangeState(new AttackState(_gunnerController));
        }
    }

    public void Exit()
    {
        Debug.Log("Exiting Move State");
    }
}
public class AttackState : IState
{
    private GunnerController _gunnerController;

    public AttackState(GunnerController gunnerController)
    {
        _gunnerController = gunnerController;
    }

    public void Enter()
    {
        if (_gunnerController.IsPlayerBehind())
        {
            _gunnerController.Animator.SetTrigger("BackAttack");
        }
        else if (_gunnerController.IsCloseEnoughToAttack())
        {
            _gunnerController.Animator.SetTrigger("CloseAttack");
        }
        else
        {
            if (Random.Range(0, 2) == 0)
            {
                _gunnerController.Animator.SetTrigger("AttackA");
            }
            else
            {
                _gunnerController.Animator.SetTrigger("AttackB");
            }
        }
    }

    public void Execute()
    {
        if (_gunnerController.IsOutOfAmmo())
        {
            _gunnerController.ChangeState(new ReloadState(_gunnerController));
        }
    }

    public void Exit() { }
}
public class ReloadState : IState
{
    private GunnerController _gunnerController;

    public ReloadState(GunnerController gunnerController)
    {
        _gunnerController = gunnerController;
    }

    public void Enter()
    {
        _gunnerController.Animator.SetTrigger("Reload");
        _gunnerController.StartCoroutine(ReloadCoroutine());
    }

    public void Execute() { }

    public void Exit() { }

    private IEnumerator ReloadCoroutine()
    {
        yield return new WaitForSeconds(_gunnerController.enemyParams.ReloadTime);
        _gunnerController.ReloadAmmo();
        _gunnerController.ChangeState(new IdleState(_gunnerController));
    }
}
public class LandingState : IState
{
    private GunnerController _gunnerController;

    public LandingState(GunnerController gunnerController)
    {
        _gunnerController = gunnerController;
    }

    public void Enter()
    {
        _gunnerController.Animator.SetTrigger("Landing");
    }

    public void Execute() { }

    public void Exit() { }
}
