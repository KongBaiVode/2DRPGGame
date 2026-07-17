using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2JumpState : Player2State
{
    public Player2JumpState(Player2 _player, Player2StateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        rb.velocity = new Vector2(rb.velocity.x, player.jumpForce);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if(rb.velocity.y < 0)
            stateMachine.ChangeState(player.airState);
    }
}
