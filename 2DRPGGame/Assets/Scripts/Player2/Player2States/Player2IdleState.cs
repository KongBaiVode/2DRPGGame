using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2IdleState : Player2GroundedState
{
    public Player2IdleState(Player2 _player, Player2StateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        
        //解决给玩家碰撞器添加SlippyMat后松开移动键出现滑行的问题，玩家会停止且不会改变方向
        player.SetZeroVelocity();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if(xInput != 0 && !player.isBusy)
            stateMachine.ChangeState(player.runStartState);
    }
}
