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

        if(xInput != 0)
        {
            player.SetVelocity(xInput * player.moveSpeed * player.inAirMoveMultiplier, rb.velocity.y);
            player.HandleFlip(xInput);
        }
    }
}
