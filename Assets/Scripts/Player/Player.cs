using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Player : Entity
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
    public float jumpForce;

    [Header("Dash Info")]
    public float dashSpeed;
    //Dash的持续时间
    public float dashDuration;
    public float dashDir { get; private set; }


    public SkillManager skill { get; private set; }



    #region States 玩家状态
    //玩家状态机
    public PlayerStateMachine stateMachine { get; private set; }


    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }

    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerWallSlideState wallSlide { get; private set; }
    public PlayerWallJumpState wallJump { get; private set; }
    public PlayerDashState dashState { get; private set; }

    public PlayerPrimaryAttackState primaryAttack { get; private set; }
    public PlayerCounterAttackState counterAttack { get; private set; }

    public PlayerAimSwordState aimSwordState { get; private set; }
    public PlayerCatchSwordState catchSwordState { get; private set; }


    #endregion




    protected override void Awake()
    {
        base.Awake();

        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState  = new PlayerAirState(this, stateMachine, "Jump");
        wallSlide = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJump  = new PlayerWallJumpState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");

        primaryAttack = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        counterAttack = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");

        aimSwordState = new PlayerAimSwordState(this, stateMachine, "AimSword");
        catchSwordState = new PlayerCatchSwordState(this, stateMachine, "CatchSword");
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

        CheckForDashInput();
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

    private void CheckForDashInput()
    {
        //dashUsageTime -= Time.deltaTime;

        // //玩家面向墙壁时无法Dash
        // if(wallDetected)
        //     return;

        //if (Input.GetKeyDown(KeyCode.LeftShift) && dashUsageTime < 0)
        if (Input.GetKeyDown(KeyCode.LeftShift) && SkillManager.Instance.dash.CanUseSkill())
        {
            //dashUsageTime = dashCooldownTime;

            dashDir = Input.GetAxisRaw("Horizontal");

            if(dashDir == 0)
                dashDir = facingDir;

            stateMachine.ChangeState(dashState);
        }
    }
}
