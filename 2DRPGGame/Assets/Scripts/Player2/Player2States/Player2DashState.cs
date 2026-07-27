using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2DashState : Player2State
{
    //玩家原始的重力缩放
    private float originalGravityScale;
    private int dashDir;

    public Player2DashState(Player2 _player, Player2StateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //克隆一个分身，在Dash时的位置处
        //SkillManager.Instance.clone.CreateClone(player.transform);
        //player.skill.clone.CreateClone(player.transform);

        dashDir = xInput != 0 ? ((int)xInput) : player.facingDir;

        stateTimer = player.dashDuration;

        //记录玩家原始的重力缩放
        originalGravityScale = rb.gravityScale;
        rb.gravityScale = 0; //即使我们在Dash状态时y轴的速度设置为零，这样可以防止玩家会因为重力的影响而仍然向下移动
    }

    public override void Exit()
    {
        base.Exit();
        //玩家冲刺结束后速度设置为0
        player.SetVelocity(0, 0);

        rb.gravityScale = originalGravityScale;
    }

    public override void Update()
    {
        base.Update();

        CancelDashIfNeeded();

        player.SetVelocity(player.dashSpeed * dashDir, 0);
        player.HandleFlip(dashDir);

        if(stateTimer < 0)
        {
            if(player.groundDetected)
                stateMachine.ChangeState(player.idleState);
            else
                stateMachine.ChangeState(player.fallState);
        }
    }

    private void CancelDashIfNeeded()
    {
        if (player.wallDetected)
        {
            if(player.groundDetected)
                stateMachine.ChangeState(player.idleState);
            else
                stateMachine.ChangeState(player.wallSlideState);
        }
    }
}
