using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2StateMachine
{
    public Player2State currentState {get; private set;}

    public void Initialize(Player2State _startState)
    {
        currentState = _startState;
        currentState.Enter();
    }

    public void ChangeState(Player2State _newState)
    {
        currentState.Exit();
        currentState = _newState;
        currentState.Enter();
    }

    public void UpdateActiveState()
    {
        currentState.Update();
    }
}
