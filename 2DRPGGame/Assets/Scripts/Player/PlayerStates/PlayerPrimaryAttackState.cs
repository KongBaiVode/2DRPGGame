using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{   
    //当前连招的标号
    private int comboCounter;

    //上一次攻击的时间
    private float lastTimeAttacked;
    //可连招窗口，该时间段结束后，下一次攻击会重置为第一段攻击
    private float comboWindow = 2f;

    public PlayerPrimaryAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //xInput = 0;  //修复Bug：玩家攻击方向偶尔反向的问题，本代码第40行。但是这样改，玩家在攻击过程中就无法改变方向了。
        xInput = Input.GetAxisRaw("Horizontal");//这样改更好，玩家可以在攻击过程中改变方向。

        if(comboCounter > 2 || Time.time > lastTimeAttacked + comboWindow)
            comboCounter = 0;

        player.animator.SetInteger("ComboCounter", comboCounter);
        //player.animator.speed = 1.2f;

        player.attackNum = comboCounter;


        #region  选择攻击方向
        float attackDir = player.facingDir;

        if(xInput != 0)
        {
            Debug.Log(player.facingDir);
            attackDir = xInput;
        }

        #endregion


        //每段攻击产生不同程度的位移
        player.SetVelocity(player.attackMovement[comboCounter].x * attackDir, player.attackMovement[comboCounter].y);


        //实现玩家在移动时攻击，会有一点惯性移动一些距离后停在原地
        stateTimer = 0.1f;
    }

    public override void Exit()
    {
        base.Exit();

        //利用协程实现0.15秒的攻击硬直
        player.StartCoroutine("BusyFor", 0.15f);
        //player.animator.speed = 1f;

        comboCounter++;
        lastTimeAttacked = Time.time;
    }

    public override void Update()
    {
        base.Update();
        
        if(stateTimer < 0)//这个if语句的作用是实现玩家在移动时攻击，会有一点惯性移动一些距离后停在原地，同时也可以防止玩家在某些状态下直接停止移动，比如冲刺
            player.SetZeroVelocity();

        if(triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }
}
