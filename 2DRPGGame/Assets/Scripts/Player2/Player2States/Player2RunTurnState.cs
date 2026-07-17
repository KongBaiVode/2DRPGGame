using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2RunTurnState : Player2State
{
    private float targetDirection; // 记录玩家想转向的新方向

    public Player2RunTurnState(Player2 _player, Player2StateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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

        // 1. 转身物理：此时玩家按着反方向，角色应该有一个减速滑行的过程
        // 我们可以重用急停的摩擦力函数
        player.ApplyStopFriction();

        // 2. 手感优化：如果转身中途玩家突然又松开了按键，直接进急停
        if (xInput == 0)
        {
            stateMachine.ChangeState(player.runStopState);
            return;
        }

        // 3. 动画播完到转身（视觉上角色已经转过身来了）
        if (triggerCalled)
        {
            // 正式翻转图片朝向
            player.HandleFlip(xInput);
            
            triggerCalled = false;

            // 丝滑切回跑步状态，由于此时按键依然按着，下一帧就会继续往新方向跑
            stateMachine.ChangeState(player.runState);
        }
    }
}
