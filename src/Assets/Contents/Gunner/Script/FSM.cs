using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM : MonoBehaviour
{
    private IState _currentState;

    public void SetState(IState newState)
    {
        if (_currentState != null)
        {
            _currentState.Exit();
        }

        _currentState = newState;
        _currentState.Enter();
    }

    public void Update()
    {
        if (_currentState != null)
        {
            _currentState.Execute();
        }
    }

    public IState GetCurrentState()
    {
        return _currentState;
    }
}
