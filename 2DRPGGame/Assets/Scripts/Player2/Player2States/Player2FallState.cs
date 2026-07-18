using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2FallState : Player2AirState
{
    public Player2FallState(Player2 _player, Player2StateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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

        // 在下落期间，如果还没二段跳，依然允许触发二段跳
        if (Input.GetKeyDown(KeyCode.Space) && !player.isDoubleJumped)
        {
            stateMachine.ChangeState(player.jump2State);
            return;
        }

        // 落地分流拦截：检测是否踩到地面
        if (player.IsGroundDetected() && rb.velocity.y <= 0.1f)
        {
            if (player.isDoubleJumped && xInput != 0)
            {
                // 如果是二段跳掉下来的，翻滚落地
                stateMachine.ChangeState(player.land2State);
            }
            else
            {
                // 普通一段跳掉下来的，普通落地
                stateMachine.ChangeState(player.land1State);
            }
        }


        // if(player.IsGroundDetected())
        //     stateMachine.ChangeState(player.idleState);
    }
}
