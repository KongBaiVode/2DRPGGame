using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2AirState : Player2State
{
    public Player2AirState(Player2 _player, Player2StateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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

        if(player.IsWallDetected())
            stateMachine.ChangeState(player.wallSlide);

        if(player.IsGroundDetected())
            stateMachine.ChangeState(player.idleState);

        if(xInput != 0)
        {
            player.SetVelocity(player.moveSpeed * 0.8f * xInput, rb.velocity.y);
            player.HandleFlip(xInput);
        }
    }
}
