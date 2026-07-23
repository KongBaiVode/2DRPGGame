using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Player2BasicAttackState : Player2State
{   
    //攻击时位移的计时器
    private float attackVelocityTimer;

    public Player2BasicAttackState(Player2 _player, Player2StateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        GenerateAttackVelocity();
    }

    public override void Exit()
    {
        base.Exit();

        
    }

    public override void Update()
    {
        base.Update();
        
        HandleAttackVelocity();

        //检测并对敌人造成伤害
        

        if(triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }

    //处理玩家攻击时的移动速度
    private void HandleAttackVelocity()
    {
        attackVelocityTimer -= Time.deltaTime;

        if(attackVelocityTimer < 0)
            player.SetVelocity(0, rb.velocity.y);
    }

    //玩家一开始攻击时产生位移的速度
    private void GenerateAttackVelocity()
    {
        //赋予计时器攻击时位移的时间
        attackVelocityTimer = player.attackVelocityDuration;
        //设置玩家攻击时的位移
        player.SetVelocity(player.attackVelocity.x * player.facingDir, player.attackVelocity.y);
    }
}
