using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonAttackState : EnemyState
{
    private Enemy_Skeleton enemy;

    public SkeletonAttackState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();

        //记录攻击结束的时间
        enemy.lastTimeAttacked = Time.time;
    }

    public override void Update()
    {
        base.Update();

        enemy.SetZeroVelocity();

        //攻击完毕，回到Battle状态，仍然具有攻击性
        if(triggerCalled)
            stateMachine.ChangeState(enemy.battleState);
    }
}
