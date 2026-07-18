using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2Jump1State : Player2AirState
{
    public Player2Jump1State(Player2 _player, Player2StateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.isDoubleJumped = false;

        player.SetVelocity(rb.velocity.x, player.jumpForce);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        // 1. 每帧把刚体的实时 Y 轴速度，喂给 Animator 的混合树参数
        // 2. 空中移动与翻转
        base.Update();

        // 3. 二段跳拦截
        if(Input.GetKeyDown(KeyCode.Space) && !player.isDoubleJumped)
        {
            stateMachine.ChangeState(player.jump2State);
            return;
        }


        if(rb.velocity.y < 0)
            stateMachine.ChangeState(player.fallState);
    }
}
