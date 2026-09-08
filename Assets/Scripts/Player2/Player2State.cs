using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player2State
{
    protected Player2StateMachine stateMachine;
    protected Player2 player;

    protected Rigidbody2D rb;
    protected Animator animator;


    protected float xInput;
    protected float yInput;
    //动画状态机中控制状态的变量名
    protected string animBoolName;

    //状态计时器
    protected float stateTimer;
    //攻击状态结束事件回调是否触发
    protected bool triggerCalled;


    public Player2State(Player2 _player, Player2StateMachine _stateMachine, string _animBoolName)
    {
        this.stateMachine = _stateMachine;
        this.player = _player;
        this.animBoolName = _animBoolName;

        animator = player.animator;
        rb = player.rb;
    }

    //进入状态
    public virtual void Enter()
    {
        // 【核心修复】：确保 Enter 的一瞬间，局部 xInput 就拿到了当帧最真实的值，摆脱 0 的命运
        xInput = Input.GetAxisRaw("Horizontal");

        //设置动画参数
        animator.SetBool(animBoolName, true);

        triggerCalled = false;
    }

    //持续在这个状态中
    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;

        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        //设置动画参数
        animator.SetFloat("yVelocity", rb.velocity.y);


        //Dash状态应该可以从任何状态转换过来，所以在这里写检测Dash的输入
        if (Input.GetKeyDown(KeyCode.LeftShift) && CanDash())
            stateMachine.ChangeState(player.dashState);
    }

    //退出状态
    public virtual void Exit()
    {
        //设置动画参数
        animator.SetBool(animBoolName, false);
    }

    public virtual void CallAnimationTrigger()
    {
        triggerCalled = true;
    }

    private bool CanDash()
    {
        if(player.wallDetected)
            return false;

        if(stateMachine.currentState == player.dashState)
            return false;


        return true;
    }
}
