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
            _gunnerController.PerformAttack();
        }
    }

    public void Exit() { }
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
            ExecuteBackAttack();
        }
        else if (_gunnerController.IsCloseEnoughToAttack())
        {
            ExecuteCloseAttack();
        }
        else
        {
            ExecuteRangedAttack();
        }
    }

    public void Execute()
    {
        if (_gunnerController.IsOutOfAmmo())
        {
            _gunnerController.ChangeState(new ReloadState(_gunnerController));
        }
        else if (!_gunnerController.IsPlayerInRange())
        {
            _gunnerController.ChangeState(new IdleState(_gunnerController));
        }
        else if (IsCurrentAnimationFinished())
        {
            _gunnerController.ChangeState(new AttackState(_gunnerController));
        }
    }

    public void Exit()
    {
        Debug.Log("Exiting AttackState");
    }

    private void ExecuteBackAttack()
    {
        _gunnerController.FacePlayer();
        _gunnerController.Animator.SetTrigger("BackAttack");
        ApplyMovement(true); 
        Debug.Log("Executing BackAttack with forward movement");
    }

    private void ExecuteCloseAttack()
    {
        _gunnerController.Animator.SetTrigger("CloseAttack");
        ApplyMovement(true); 
        Debug.Log("Executing CloseAttack with forward movement");
    }

    private void ExecuteRangedAttack()
    {
        int attackType = Random.Range(0, 2);
        if (attackType == 0)
        {
            _gunnerController.Animator.SetTrigger("AttackA");
            ApplyMovement(false); 
            Debug.Log("Executing AttackA with backward movement");
        }
        else
        {
            _gunnerController.Animator.SetTrigger("AttackB");
            ApplyMovement(false);
            Debug.Log("Executing AttackB with backward movement");
        }
    }

    private bool IsCurrentAnimationFinished()
    {
        AnimatorStateInfo stateInfo = _gunnerController.Animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.normalizedTime >= 1.0f && !stateInfo.loop; 
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
