using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2Jump2State : Player2AirState
{
    public Player2Jump2State(Player2 _player, Player2StateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.isDoubleJumped = true; // 标记已交二段跳

        // Y轴速度清零并施加二段跳冲力
        player.SetVelocity(rb.velocity.x, 0); 
        player.SetVelocity(rb.velocity.x, player.jumpForce);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        // 空中左右微调
        base.Update();

        // 一旦 Y 轴速度小于 0（开始下落）
        if (rb.velocity.y < 0)
        {
            // 直接扔回通用的空中状态，让空中的混合树去自动解算下落画面！
            stateMachine.ChangeState(player.fallState);
        }
    }
}
