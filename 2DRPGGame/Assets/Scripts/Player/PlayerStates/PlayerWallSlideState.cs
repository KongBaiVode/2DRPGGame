using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallSlideState : PlayerState
{
    public PlayerWallSlideState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            stateMachine.ChangeState(player.wallJump);
            return;//这里添加return是因为执行完这个if语句后会进入if(yInput < 0)-else语句快，导致玩家在X轴方向上的速度又被设置为0，使玩家跳墙跳不出去
        }

        if(yInput < 0)
            rb.velocity = new Vector2(0, rb.velocity.y);
        else
            rb.velocity = new Vector2(0, rb.velocity.y * 0.7f);

        if((xInput != 0 && player.facingDir != xInput) || !player.IsWallDetected()) //这里添加!player.IsWallDetected()判断是解决在玩家在AirState状态下碰到墙壁的瞬间按反方向的移动键后玩家的滑墙方向改变的Bug
            stateMachine.ChangeState(player.idleState);//这里切换回idleState是因为idleState状态是万能的，可以转变为许多其他状态，比如airState
        
        if(player.IsGroundDetected())
            stateMachine.ChangeState(player.idleState);
    }
}
