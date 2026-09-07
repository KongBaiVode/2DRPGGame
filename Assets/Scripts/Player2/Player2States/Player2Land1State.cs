using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2Land1State : Player2State
{
    public Player2Land1State(Player2 _player, Player2StateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.isDoubleJumped = false;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        //鼠标左键点击，进入攻击状态
        if(Input.GetKey(KeyCode.Mouse0))
            stateMachine.ChangeState(player.basicAttackState);

        //允许玩家用移动或再次跳跃来强制打断落地动画
        if(Input.GetKeyDown(KeyCode.Space))
            stateMachine.ChangeState(player.jump1State);

        if (xInput != 0) 
        { 
            stateMachine.ChangeState(player.runStartState); 
            return; 
        }

        player.SetVelocity(0, rb.velocity.y); // 普通落地时原地产生短暂轻微硬直，速度清零

        if (triggerCalled)
        {
            stateMachine.ChangeState(player.idleState); // 播完回 Idle（如果按着方向键，Idle下一帧会自动进Run）
        }
    }
}
