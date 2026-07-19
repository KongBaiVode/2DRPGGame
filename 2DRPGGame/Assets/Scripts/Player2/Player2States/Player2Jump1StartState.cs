using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2Jump1StartState : Player2GroundedState
{
    public Player2Jump1StartState(Player2 _player, Player2StateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if(triggerCalled)
            stateMachine.ChangeState(player.jump1State);
    }
}
