using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Player2 : Entity
{   
    [Header("Attack details")]
    public Vector2[] attackMovement;
    //我们还可以像冲刺一样维护一个attackDir，然后在GroundState中替换掉attack输入，这样我们就可以在攻击时改变攻击方向
    //玩家反击持续时间
    public float counterAttackDuration = 0.2f;
    

    //用于实现玩家攻击产生硬直（后摇）
    public bool isBusy;

    //玩家移动
    [Header("Move Info")]
    public float moveSpeed = 12f;
    [Range(0, 1)]
    public float inAirMoveMultiplier = 0.7f;
    [Range(0, 1)]
    public float wallSlideMultiplier = 0.7f;
    public float stopFriction; //急停时的摩擦力
    public float jumpForce;
    [HideInInspector] public bool isDoubleJumped; // 记录当前是否已经二段跳
    public Vector2 wallJumpForce;

    [Header("Dash Info")]
    public float dashSpeed;
    //Dash的持续时间
    public float dashDuration;
    public float dashDir;


    public SkillManager skill { get; private set; }



    #region States 玩家状态
    //玩家状态机
    public Player2StateMachine stateMachine { get; private set; }


    public Player2IdleState idleState { get; private set; }
    
    public Player2RunStartState runStartState { get; private set; }
    public Player2RunState runState { get; private set; }
    public Player2RunStopState runStopState { get; private set; }
    public Player2RunTurnState runTurnState { get; private set; }

    public Player2Jump1StartState jump1StartState { get; private set; }
    public Player2Jump1State jump1State { get; private set; }
    public Player2Jump2State jump2State { get; private set; }
    public Player2Land1State land1State { get; private set; }
    public Player2Land2State land2State { get; private set; }
    public Player2FallState fallState { get; private set; }

    public Player2WallSlideState wallSlideState { get; private set; }
    public Player2WallJumpStartState wallJumpStartState { get; private set; }
    public Player2WallJumpState wallJumpState { get; private set; }

    public Player2DashState dashState { get; private set; }

    public Player2PrimaryAttackState primaryAttack { get; private set; }
    public Player2CounterAttackState counterAttack { get; private set; }


    #endregion




    protected override void Awake()
    {
        base.Awake();

        stateMachine = new Player2StateMachine();


        idleState = new Player2IdleState(this, stateMachine, "Idle");

        runStartState = new Player2RunStartState(this, stateMachine, "RunStart");
        runState = new Player2RunState(this, stateMachine, "Run");
        runStopState = new Player2RunStopState(this, stateMachine, "RunStop");
        runTurnState = new Player2RunTurnState(this, stateMachine, "RunTurn");

        jump1StartState = new Player2Jump1StartState(this, stateMachine, "Jump1Start");
        jump1State = new Player2Jump1State(this, stateMachine, "Jump1");
        jump2State = new Player2Jump2State(this, stateMachine, "Jump2");
        land1State = new Player2Land1State(this, stateMachine, "Land1");
        land2State = new Player2Land2State(this, stateMachine, "Land2");
        fallState  = new Player2FallState(this, stateMachine, "Jump1");

        wallSlideState = new Player2WallSlideState(this, stateMachine, "WallSlide");
        wallJumpStartState = new Player2WallJumpStartState(this, stateMachine, "WallJumpStart");
        wallJumpState  = new Player2WallJumpState(this, stateMachine, "WallJump");

        dashState = new Player2DashState(this, stateMachine, "Dash");

        primaryAttack = new Player2PrimaryAttackState(this, stateMachine, "Attack");
        counterAttack = new Player2CounterAttackState(this, stateMachine, "CounterAttack");

    }

    protected override void Start()
    {
        base.Start();

        skill = SkillManager.Instance;

        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();

        stateMachine.UpdateActiveState();

        //CheckForDashInput();
    }

    public IEnumerator BusyFor(float _second)
    {
        isBusy = true;
        //Debug.Log("Is Busy");

        yield return new WaitForSeconds(_second);

        isBusy = false;
        //Debug.Log("Not Busy");
    }

    //状态结束后动画事件触发。这样写可以让其他函数调用玩家组件中的这个方法，且可以获得对应状态的该虚方法的实例
    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();

    // private void CheckForDashInput()
    // {
    //     //dashUsageTime -= Time.deltaTime;

    //     // //玩家面向墙壁时无法Dash
    //     // if(wallDetected)
    //     //     return;

    //     //if (Input.GetKeyDown(KeyCode.LeftShift) && dashUsageTime < 0)
    //     if (Input.GetKeyDown(KeyCode.LeftShift) && SkillManager.Instance.dash.CanUseSkill())
    //     {
    //         //dashUsageTime = dashCooldownTime;

    //         dashDir = Input.GetAxisRaw("Horizontal");

    //         if(dashDir == 0)
    //             dashDir = facingDir;

    //         stateMachine.ChangeState(dashState);
    //     }
    // }

    //RunStop状态调用的急停函数
    public void ApplyStopFriction()
    {
        // 急停时，让水平速度以插值方式滑行衰减至0
        float newX = Mathf.MoveTowards(rb.velocity.x, 0, stopFriction * Time.deltaTime * 100);
        rb.velocity = new Vector2(newX, rb.velocity.y);
    }
}
