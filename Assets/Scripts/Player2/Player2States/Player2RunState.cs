using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2RunState : Player2GroundedState
{
    public Player2RunState(Player2 _player, Player2StateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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

        if(xInput == 0)
        {
            stateMachine.ChangeState(player.runStopState);
            return;
        }

        if(xInput != 0 && xInput != player.facingDir)
        {
            stateMachine.ChangeState(player.runTurnState);
            return;
        }

        player.SetVelocity(xInput * player.moveSpeed, rb.velocity.y);
    }
}
