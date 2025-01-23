using UnityEngine;
using System.Collections;
using Feature.Component;
using Core.Utilities;

public class IdleState : IState
{
    private GunnerController _gunnerController;
    private float coldTimer;

    public IdleState(GunnerController gunnerController)
    {
        _gunnerController = gunnerController;
    }

    public void Enter()
    {
        _gunnerController.Animator.SetBool("Idle", true);
        coldTimer = Time.time;
    }

    public void Execute()
    {
        GunnerThink();
    }

    public void Exit()
    {
        _gunnerController.Animator.SetBool("Idle", false);
    }

    private void GunnerThink()
    {
        if (_gunnerController.IsPlayerBehind())
        {
            _gunnerController.ChangeState(new BackAttackState(_gunnerController));
            _gunnerController.attackCount++;
        }
        else if (_gunnerController.GetCurrentAttackCount() >= _gunnerController.enemyParams.SpecialAttackAmmo)
        {
            _gunnerController.ChangeState(new SpecialAttackState(_gunnerController));
            _gunnerController.attackCount = 0;
            _gunnerController.specialAttackCount++;
        }
        else if (_gunnerController.GetCurrentSpecialAttackCount() >= _gunnerController.enemyParams.FlyAttackAmmo)
        {
            _gunnerController.ChangeState(new FlyAttackState(_gunnerController));
            _gunnerController.specialAttackCount = 0;
        }
        else if (Time.time - coldTimer >= _gunnerController.enemyParams.BasicAttackCooldown)
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

public class FaceAttackState : IState
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
            _gunnerController.ChangeState(new IdleState(_gunnerController));
        }
    }

    public void Exit()
    {
        _gunnerController.Animator.SetBool("CloseAttack", false);
    }

    private void ExecuteCloseAttack()
    {
        _gunnerController.Animator.SetBool("CloseAttack", true);
        Debug.Log("Executing CloseAttack with forward movement");
    }
}

public class MidDistanceAttackState : IState
{
    private GunnerController _gunnerController;

    public MidDistanceAttackState(GunnerController gunnerController)
    {
        _gunnerController = gunnerController;
    }

    public void Enter()
    {
        ExecuteRangedAttack();
    }

    public void Execute()
    {
        if (_gunnerController.IsCurrentAnimationFinished())
        {
            _gunnerController.attackCount++;
            if (_gunnerController.attackCount >= 3)
            {
                _gunnerController.ChangeState(new BeamAttackState(_gunnerController));
                _gunnerController.attackCount = 0;
            }
            else
            {
                _gunnerController.ChangeState(new MidDistanceAttackStateB(_gunnerController));
            }
        }
    }

    public void Exit()
    {
        _gunnerController.Animator.SetBool("AttackA", false);
    }

    private void ExecuteRangedAttack()
    {
        _gunnerController.Animator.SetBool("AttackA", true);
        Debug.Log("Executing AttackA with backward movement");
    }
}

public class MidDistanceAttackStateB : IState
{
    private GunnerController _gunnerController;

    public MidDistanceAttackStateB(GunnerController gunnerController)
    {
        _gunnerController = gunnerController;
    }

    public void Enter()
    {
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
        _gunnerController.Animator.SetBool("AttackB", false);
    }

    private void ExecuteRangedAttack()
    {
        _gunnerController.Animator.SetBool("AttackB", true);
        Debug.Log("Executing AttackB with backward movement");
    }
}

public class FarDistanceAttackState : IState
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
        if (_gunnerController.IsCurrentAnimationFinished())
        {
            _gunnerController.ChangeState(new IdleState(_gunnerController));
        }
    }

    public void Exit()
    {
        _gunnerController.Animator.SetBool("AttackFC", false);
    }
}

public class OverDistanceAttackState : IState
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
        if (_gunnerController.IsCurrentAnimationFinished())
        {
            _gunnerController.ChangeState(new FarDistanceAttackState(_gunnerController));
        }
    }

    public void Exit()
    {
        _gunnerController.Animator.SetBool("FarAttack", false);
    }
}

public class SpecialAttackState : IState
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
        if (_gunnerController.IsCurrentAnimationFinished())
        {
            _gunnerController.ChangeState(new LandingState(_gunnerController));
        }
    }

    public void Exit()
    {
        _gunnerController.Animator.SetBool("SpecialAttack", false);
    }
}

public class BackAttackState : IState
{
    private GunnerController _gunnerController;

    public BackAttackState(GunnerController gunnerController)
    {
        _gunnerController = gunnerController;
    }

    public void Enter()
    {
        ExecuteBackAttack();
    }

    public void Execute()
    {
        if (IsCurrentAnimationFinished())
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
        return stateInfo.normalizedTime >= 1.0f && !stateInfo.loop;
    }

    private void ExecuteBackAttack()
    {
        _gunnerController.Animator.SetBool("Back", true);
        Debug.Log("Executing BackAttack with forward movement");
    }
}

public class FlyAttackState : IState
{
    private GunnerController _gunnerController;

    public FlyAttackState(GunnerController gunnerController)
    {
        _gunnerController = gunnerController;
    }

    public void Enter()
    {
        _gunnerController.ShootFlyRayBullet();
    }

    public void Execute()
    {
        _gunnerController.ChangeState(new LandingState(_gunnerController));
    }

    public void Exit()
    {
        Debug.Log("Exiting FlyAttackState");
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
        _gunnerController.Animator.SetBool("Landing", true);
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
        _gunnerController.Animator.SetBool("Landing", false);
    }
}

public class BeamAttackState : IState
{
    private GunnerController _gunnerController;

    public BeamAttackState(GunnerController gunnerController)
    {
        _gunnerController = gunnerController;
    }

    public void Enter()
    {
        _gunnerController.Animator.SetTrigger("BeamAttack");
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
    }
}
