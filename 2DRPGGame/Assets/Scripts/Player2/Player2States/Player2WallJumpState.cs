using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2WallJumpState : Player2State
{
    public Player2WallJumpState(Player2 _player, Player2StateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = 0.4f;
        player.SetVelocity(5 * -player.facingDir, player.jumpForce);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if(stateTimer < 0)
            stateMachine.ChangeState(player.airState);

        if(player.IsGroundDetected())
            stateMachine.ChangeState(player.idleState);
    }
}
