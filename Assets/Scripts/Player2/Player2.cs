using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

//攻击判定形状枚举
public enum AttackShape
{
    Circle,  //圆形（适合环形爆发、小范围近身攻击）
    Box,     //矩形（适合横扫、刺激、纵劈）
    Capsule  //胶囊体（适合斜切、弧形斩击）
}
//攻击数据结构
[System.Serializable]
public struct AttackData
{
    [Header("位移参数")]
    public Vector2 moveVelocity;    //该段攻击的移动速度
    public float moveDuration;      //该段攻击位移持续时间

    [Header("攻击判定形状与范围")]
    public AttackShape shapeType;   //攻击判定形状
    public Vector2 attackOffset;    //中心偏移量

    //根据 shapeType 选择填入以下参数：
    public float attackRadius;      //圆形半径
    public Vector2 attackSize;      //矩形/胶囊体的宽高
    public float attackAngle;       //矩形、胶囊体的旋转角度（用于倾斜斩击）

    [Header("伤害数值")]
    public float damage;            //该段攻击的伤害
}

public class Player2 : Entity
{   
    [Header("Attack details")]
    //玩家攻击配置
    public AttackData[] attackDatas;
    //连招次数重置时间
    public float comboResetTime = 1f;
    private Coroutine queuedAttackCoroutine;     //如果在攻击状态中点击了攻击键，则使用协程在下一帧直接切换为攻击状态，避免在同一帧内切换导致切换状态无效
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

    public Player2BasicAttackState basicAttackState { get; private set; }
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

        basicAttackState = new Player2BasicAttackState(this, stateMachine, "BasicAttack");
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

    public void EnterAttackStateWithDelay()
    {
        //防止点击了多次攻击按键导致开启了多个协程（会使玩家不停攻击多次）
        if(queuedAttackCoroutine != null)
            StopCoroutine(queuedAttackCoroutine);

        queuedAttackCoroutine = StartCoroutine(EnterAttackStateWithDelayCoroutine());
    }

    private IEnumerator EnterAttackStateWithDelayCoroutine()
    {
        yield return new WaitForEndOfFrame();
        stateMachine.ChangeState(basicAttackState);
    }

    protected override void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance * facingDir, wallCheck.position.y));


        if (attackCheck == null || attackDatas == null) return;

        Color[] colors = { Color.red, Color.yellow, Color.green, Color.cyan, Color.magenta };

        for (int i = 0; i < attackDatas.Length; i++)
        {
            Gizmos.color = colors[i % colors.Length];
            AttackData data = attackDatas[i];

            // 计算世界坐标偏移（考虑面向 facingDir）
            Vector3 checkPosition = attackCheck.position + new Vector3(data.attackOffset.x * facingDir, data.attackOffset.y, 0);

            float finalAngle = data.attackAngle * facingDir;

            switch (data.shapeType)
            {
                case AttackShape.Circle:
                    Gizmos.DrawWireSphere(checkPosition, data.attackRadius);
                    break;

                case AttackShape.Box:
                    // 保存原有的 Gizmos 矩阵，避免影响后续绘制
                    Matrix4x4 oldMatrix = Gizmos.matrix;

                    // 计算带面向和角度的旋转矩阵
                    
                    Gizmos.matrix = Matrix4x4.TRS(checkPosition, Quaternion.Euler(0, 0, finalAngle), Vector3.one);

                    // 绘制矩形框
                    Gizmos.DrawWireCube(Vector3.zero, data.attackSize);

                    // 恢复 Gizmos 矩阵
                    Gizmos.matrix = oldMatrix;
                    break;
                case AttackShape.Capsule:
                // 【核心修复】：拆分 Capsule 逻辑，呼叫专用的 2D 胶囊体绘制辅助函数
                DrawWireCapsule2D(checkPosition, data.attackSize, CapsuleDirection2D.Vertical, finalAngle);
                break;
            }
        }
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
    public void CallAnimationTrigger() => stateMachine.currentState.CallAnimationTrigger();

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





    #region Gizmos 辅助绘制工具

    /// <summary>
    /// 在 Scene 视图中绘制 2D 胶囊体线框
    /// </summary>
    private void DrawWireCapsule2D(Vector3 position, Vector2 size, CapsuleDirection2D direction, float angle)
    {
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(position, Quaternion.Euler(0, 0, angle), Vector3.one);

        float width = size.x;
        float height = size.y;

        if (direction == CapsuleDirection2D.Vertical)
        {
            // 如果高度小于等于宽度，胶囊体会退化为圆形
            if (height <= width)
            {
                Gizmos.DrawWireSphere(Vector3.zero, width / 2f);
            }
            else
            {
                float radius = width / 2f;
                float halfCapHeight = (height - width) / 2f;

                // 1. 绘制左右两条连接线
                Gizmos.DrawLine(new Vector3(-radius, -halfCapHeight, 0), new Vector3(-radius, halfCapHeight, 0));
                Gizmos.DrawLine(new Vector3(radius, -halfCapHeight, 0), new Vector3(radius, halfCapHeight, 0));

                // 2. 绘制上下两个圆弧
                DrawWireArc(new Vector3(0, halfCapHeight, 0), radius, 0, 180);
                DrawWireArc(new Vector3(0, -halfCapHeight, 0), radius, 180, 360);
            }
        }
        else // Horizontal
        {
            if (width <= height)
            {
                Gizmos.DrawWireSphere(Vector3.zero, height / 2f);
            }
            else
            {
                float radius = height / 2f;
                float halfCapWidth = (width - height) / 2f;

                // 1. 绘制上下两条连接线
                Gizmos.DrawLine(new Vector3(-halfCapWidth, radius, 0), new Vector3(halfCapWidth, radius, 0));
                Gizmos.DrawLine(new Vector3(-halfCapWidth, -radius, 0), new Vector3(halfCapWidth, -radius, 0));

                // 2. 绘制左右两个圆弧
                DrawWireArc(new Vector3(halfCapWidth, 0, 0), radius, -90, 90);
                DrawWireArc(new Vector3(-halfCapWidth, 0, 0), radius, 90, 270);
            }
        }

        Gizmos.matrix = oldMatrix;
    }

    /// <summary>
    /// 绘制圆弧辅助线
    /// </summary>
    private void DrawWireArc(Vector3 center, float radius, float startAngle, float endAngle, int segments = 10)
    {
        float step = (endAngle - startAngle) / segments;
        Vector3 lastPoint = center + new Vector3(Mathf.Cos(startAngle * Mathf.Deg2Rad) * radius, Mathf.Sin(startAngle * Mathf.Deg2Rad) * radius, 0);

        for (int i = 1; i <= segments; i++)
        {
            float a = (startAngle + step * i) * Mathf.Deg2Rad;
            Vector3 nextPoint = center + new Vector3(Mathf.Cos(a) * radius, Mathf.Sin(a) * radius, 0);
            Gizmos.DrawLine(lastPoint, nextPoint);
            lastPoint = nextPoint;
        }
    }

    #endregion
}





