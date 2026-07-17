using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2DashState : Player2State
{
    public Player2DashState(Player2 _player, Player2StateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //克隆一个分身，在Dash时的位置处
        //SkillManager.Instance.clone.CreateClone(player.transform);
        player.skill.clone.CreateClone(player.transform);

        stateTimer = player.dashDuration;
    }

    public override void Exit()
    {
        base.Exit();
        //玩家冲刺结束后速度设置为0
        player.SetVelocity(0, rb.velocity.y);
    }

    public override void Update()
    {
        base.Update();

        if(!player.IsGroundDetected() && player.IsWallDetected())
            stateMachine.ChangeState(player.wallSlide);

        player.SetVelocity(player.dashSpeed * player.dashDir, 0);
        player.HandleFlip(player.dashDir);

        if(stateTimer < 0)
            stateMachine.ChangeState(player.idleState);
    }
}
