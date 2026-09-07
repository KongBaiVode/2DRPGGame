using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Player2BasicAttackState : Player2State
{   
    //攻击时位移的计时器
    private float attackVelocityTimer;

    private const int FirstComboIndex = 0;  //连招索引从0开始，并且这个索引会用于Animator
    private int comboIndex = 0;             //当前连招的索引号
    private int comboLimit = 4;             //连招的最大索引号

    private float lastTimeAttacked;         //上次攻击的时间，用于重置连招的索引号

    private bool comboAttackQueued;         //当前攻击结束后是否有下一段攻击在排队

    private int attackDir;                  //攻击方向

    public Player2BasicAttackState(Player2 _player, Player2StateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
        if(comboLimit != player.attackDatas.Length - 1)
        {
            Debug.LogWarning("我根据attacketDatas攻击数据调整了连招的最大索引号");
            comboLimit = player.attackDatas.Length - 1;
        }
    }

    public override void Enter()
    {
        base.Enter();

        comboAttackQueued = false;

        ResetComboIndexIfNeeded();

        //xInput = 0;  //修复Bug：玩家攻击方向偶尔反向的问题，本代码第40行。但是这样改，玩家在攻击过程中就无法改变方向了。
        //xInput = Input.GetAxisRaw("Horizontal");//这样改更好，玩家可以在攻击过程中改变方向。
        //根据输入设置攻击方向
        attackDir = xInput != 0 ? ((int)xInput) : player.facingDir;

        animator.SetInteger("BasicAttackIndex", comboIndex);

        ApplyAttackMoveVelocity();
    }


    public override void Exit()
    {
        base.Exit();

        comboIndex++;

        //记录攻击的时间
        lastTimeAttacked = Time.time;
    }

    public override void Update()
    {
        base.Update();
        
        HandleAttackVelocity();

        //检测并对敌人造成伤害
        
        //检测在此次连招内是否按了攻击键，按了则说明有下一个攻击连招在排队，本次连招结束后直接会进入下一个攻击状态，使得攻击更加连贯
        if(Input.GetKeyDown(KeyCode.Mouse0))
            QueueNextAttack();

        if (triggerCalled)
            HandleStateExit();
    }

    private void HandleStateExit()
    {
        if (comboAttackQueued)
            {
                animator.SetBool(animBoolName, false);      //如果不加这句代码，使用协程和没使用协程的结果一样
                player.EnterAttackStateWithDelay();
            }
            else
                stateMachine.ChangeState(player.idleState);
    }

    //只有当攻击不是最后一段时，才能使用协程实现攻击预输入
    private void QueueNextAttack()
    {
        if(comboIndex < comboLimit)
            comboAttackQueued = true;
    }

    //处理玩家攻击时的移动速度
    private void HandleAttackVelocity()
    {
        attackVelocityTimer -= Time.deltaTime;

        if(attackVelocityTimer < 0)
            player.SetVelocity(0, rb.velocity.y);
    }

    //玩家一开始攻击时产生位移的速度
    private void ApplyAttackMoveVelocity()
    {
        Vector2 attackVelocity = player.attackDatas[comboIndex].moveVelocity;

        //赋予计时器攻击时位移的时间
        attackVelocityTimer = player.attackDatas[comboIndex].moveDuration;
        //设置玩家攻击时的位移
        player.SetVelocity(attackVelocity.x * attackDir, attackVelocity.y);
        player.HandleFlip(attackVelocity.x * attackDir);  
        //这样写会有一个问题：玩家攻击时只能朝攻击的方向移动，不能后退。

    }

    //重置连招索引
    private void ResetComboIndexIfNeeded()
    {
        //如果上次攻击的时间到现在已经超过了comboResetTime，我们需要重置连招索引
        if(Time.time > lastTimeAttacked + player.comboResetTime)
            comboIndex = FirstComboIndex;
        //如果连招索引超过了最大连招次数，我们需要重置连招索引
        if (comboIndex > comboLimit)
            comboIndex = FirstComboIndex;
    }


}
