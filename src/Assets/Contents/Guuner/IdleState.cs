using System.Xml;
using UnityEngine;
using System.Collections;

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
    private int currentMoveCount = 0;
    private int specialMoveThreshold;

    public MoveState(GunnerController gunnerController)
    {
        _gunnerController = gunnerController;
        specialMoveThreshold = (_gunnerController.MovesBeforeSpecialMove > 0)
            ? _gunnerController.MovesBeforeSpecialMove
            : Random.Range(1, 10);
    }

    public void Enter()
    {
        Debug.Log("Entering Move State");
    }

    public void Execute()
    {
        _gunnerController.MoveTowardsPlayer();
        currentMoveCount++;

        if (currentMoveCount >= specialMoveThreshold)
        {
            _gunnerController.SpecialMove();
            currentMoveCount = 0;
            specialMoveThreshold = (_gunnerController.MovesBeforeSpecialMove > 0)
                ? _gunnerController.MovesBeforeSpecialMove
                : Random.Range(1, 10);
        }

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
    private int attackCounter = 0;

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
            if (attackCounter < 3)
            {
                _gunnerController.Attack();
                attackCounter++;
            }
            else
            {
                _gunnerController.SpecialAttack();
                attackCounter = 0;
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
        Debug.Log("Entering Reload State");
        _gunnerController.StartCoroutine(ReloadCoroutine());
    }

    public void Execute() { }

    public void Exit()
    {
        Debug.Log("Exiting Reload State");
    }

    private IEnumerator ReloadCoroutine()
    {
        yield return new WaitForSeconds(_gunnerController.ReloadTime);
        _gunnerController.ReloadAmmo();
        _gunnerController.ChangeState(new IdleState(_gunnerController));
    }
}