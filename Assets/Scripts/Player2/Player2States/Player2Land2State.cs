using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2Land2State : Player2State
{
    public Player2Land2State(Player2 _player, Player2StateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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

        if (xInput != 0 && xInput != player.facingDir) 
        { 
            stateMachine.ChangeState(player.idleState); 
            return; 
        }

        // 视觉细节：翻滚落地通常带有向前的卸力位移
        // 我们可以允许角色在翻滚时顺着当前的朝向保留一丁点向前的微弱滑行速度
        player.SetVelocity(player.facingDir * player.moveSpeed, rb.velocity.y);

        if (triggerCalled)
        {
            // 如果播完翻滚时玩家还死死按着方向键，可以直接切入 RunState，手感更连贯
            if (xInput != 0)
                stateMachine.ChangeState(player.runState);
            else
                stateMachine.ChangeState(player.idleState);
        }
    }
}
