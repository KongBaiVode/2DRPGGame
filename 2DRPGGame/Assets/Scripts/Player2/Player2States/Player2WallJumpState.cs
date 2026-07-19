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

        player.isDoubleJumped = false;

        player.SetZeroVelocity();
        player.SetVelocity(player.wallJumpForce.x * -player.facingDir, player.jumpForce);
        player.Flip();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        // 3. 二段跳拦截
        if(Input.GetKeyDown(KeyCode.Space) && !player.isDoubleJumped)
        {
            stateMachine.ChangeState(player.jump2State);
            return;
        }

        // 当墙跳上升力量用尽，开始掉落时（velocity.y < 0）
        // 直接无缝切回通用的空中状态（jump1State），让常规空中混合树去接管它的下落画面！
        if(rb.velocity.y < 0)
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }

        if(player.wallDetected)
            stateMachine.ChangeState(player.wallSlideState);

        if(player.groundDetected)
            stateMachine.ChangeState(player.land1State);
    }
}
