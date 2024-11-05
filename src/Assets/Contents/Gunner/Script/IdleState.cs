using System.Collections;
using UnityEngine;

public class IdleState : IState
{
    private GunnerController _gunnerController;

    public IdleState(GunnerController gunnerController)
    {
        _gunnerController = gunnerController;
    }

    public void Enter()
    {
        Debug.Log("Entering Idle State");
        _gunnerController.PlayAnimation("GUN_standby"); 
    }

    public void Execute()
    {
        if (_gunnerController.IsPlayerInRange())
        {
            _gunnerController.ChangeState(new MoveState(_gunnerController));
        }
    }

    public void Exit()
    {
        Debug.Log("Exiting Idle State");
    }
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
        Debug.Log("Entering Attack State");
    }

    public void Execute()
    {
        if (!_gunnerController.IsOutOfAmmo())
        {
            _gunnerController.PerformAttack();

            if (_gunnerController.ShouldPerformSpecialAttack())
            {
                _gunnerController.StartAttackCooldown(_gunnerController.enemyParams.SpecialAttackCooldown);
            }
            else
            {
                _gunnerController.StartAttackCooldown(_gunnerController.enemyParams.BasicAttackCooldown);
            }
        }
        else
        {
            _gunnerController.ChangeState(new ReloadState(_gunnerController));
        }
    }

    public void Exit()
    {
        Debug.Log("Exiting Attack State");
    }
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
        _gunnerController.StartCoroutine(ReloadCoroutine());
    }

    public void Execute() { }

    public void Exit()
    {
        Debug.Log("Exiting Reload State");
    }

    private IEnumerator ReloadCoroutine()
    {
        yield return new WaitForSeconds(_gunnerController.enemyParams.ReloadTime);
        _gunnerController.ReloadAmmo();
        _gunnerController.ChangeState(new IdleState(_gunnerController));
    }
}
