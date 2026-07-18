using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    #region Components 玩家、敌人组件
    public Animator animator { get; private set; }
    public Rigidbody2D rb { get; private set; }

    public EntityFX fx { get; private set; }

    #endregion


    [Header("Knockback Info")]
    [SerializeField] protected Vector2 knockbackDirection;
    [SerializeField] protected float knockbackDuration;
    protected bool isKnocked;


    //玩家、敌人碰撞
    [Header("Collision Info")]
    public Transform attackCheck;
    public float[] attackCheckRadius;
    public int attackNum = 0;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;
    public bool groundDetected { get; private set; }
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;
    public bool wallDetected { get; private set; }


    //玩家、敌人朝向
    public int facingDir { get; private set; } = 1;
    protected bool facingRight = true;


    protected virtual void Awake()
    {
        animator = GetComponentInChildren<Animator>(); //要先得到Animator，再创建StateMachine，然后再创建各种State，否则会报错
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        fx = GetComponent<EntityFX>();
    }

    protected virtual void Update()
    {
        IsGroundDetected();
        IsWallDetected();
    }

    public void Damage(int _facingDir)
    {
        fx.StartCoroutine("FlashFX");
        StartCoroutine("HitKnockback", _facingDir);


        Debug.Log(gameObject.name + " was damaged!");
    }

    protected virtual IEnumerator HitKnockback(int _facingDir)
    {
        isKnocked = true;

        rb.velocity = new Vector2(knockbackDirection.x * _facingDir, knockbackDirection.y);

        yield return new WaitForSeconds(knockbackDuration);
        isKnocked = false;
    }

    #region Velocity
    //设置玩家刚体的速度为0
    public void SetZeroVelocity()
    {
        if(isKnocked)
            return;
        
        rb.velocity = new Vector2(0, 0);
    }

    //设置玩家刚体的速度
    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        if(isKnocked)
            return;

        rb.velocity = new Vector2(_xVelocity, _yVelocity);
        // HandleFlip(_xVelocity);
    }
    #endregion


    #region Collision
    //地面检测
    public virtual void IsGroundDetected()
    {
        groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    }
    //public bool IsGroundDetected() => Physics2D.OverlapCircle(groundCheck.position, groundCheckDistance, whatIsGround);
    //墙体检测
    public virtual void IsWallDetected()
    {
        wallDetected = Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance * facingDir, wallCheck.position.y));
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius[attackNum]);
    }
    #endregion


    #region Flip
    public virtual void Flip()
    {
        facingDir = facingDir * -1;
        facingRight = !facingRight;
        this.transform.Rotate(0, 180, 0);
    }

    public virtual void HandleFlip(float _xVelocity)
    {
        if(_xVelocity > 0 && !facingRight)
            Flip();
        else if(_xVelocity < 0 && facingRight)
            Flip();
    }
    #endregion
}
