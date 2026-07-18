using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2RunStopState : Player2GroundedState
{
    public Player2RunStopState(Player2 _player, Player2StateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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

        player.ApplyStopFriction();

        if(xInput != 0)
        {
            // 【核心修复】：当玩家在急停期间重新按下了移动键
            // 关键判定：如果玩家按下的新方向，和角色当前的 facingDir（原跑动方向）相反
            // 说明玩家是在高速奔跑中做出了“紧急折返/倒车”操作！
            if (Mathf.Sign(xInput) != player.facingDir)
            {
                // // 立刻拦截，改道送去转身状态！
                // stateMachine.ChangeState(player.runTurnState);
                
                // 核心修复：检查当前玩家的绝对速度是否大于最大速度的 35% (阈值可自行调优)
                // 注：如果你的 rb 在 player 脚本里，请写 player.rb.velocity.x
                if (Mathf.Abs(player.rb.velocity.x) > player.moveSpeed * 0.35f)
                {
                    // 情况 A：速度还很快，允许播放华丽的急停转身动画
                    stateMachine.ChangeState(player.runTurnState);
                }
                else
                {
                    // 情况 B：速度已经很慢了，再播转身会卡顿！
                    // 直接原地强行转头，并直接进入起跑状态，手感拉满！
                    player.HandleFlip(xInput);
                    stateMachine.ChangeState(player.runStartState);
                }
            }
            else
            {
                // 如果按的是相同方向（比如松了一下键又马上接着往前跑），则正常进起跑
                stateMachine.ChangeState(player.runStartState);
            }
            return;
        }

        if(triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }
}
