using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class SkeletonMoveState : SkeletonGroundState
{
    public SkeletonMoveState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
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

        enemy.SetVelocity( enemy.moveSpeed * enemy.facingDir, rb.velocity.y);
        enemy.HandleFlip(enemy.moveSpeed * enemy.facingDir);

        if(enemy.wallDetected || !enemy.groundDetected)
        {
            enemy.Flip();
            stateMachine.ChangeState(enemy.idleState);
        }
    }
}
