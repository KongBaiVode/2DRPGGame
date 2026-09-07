using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_SkeletonAnimationTriggers : MonoBehaviour
{
    private Enemy_Skeleton enemy => GetComponentInParent<Enemy_Skeleton>();

    private void AnimationTrigger()
    {
        enemy.AnimationFinishTrigger();
    }

    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.attackCheck.position, enemy.attackCheckRadius[enemy.attackNum]);

        foreach(var hit in colliders)
        {
            if(hit.GetComponent<Player>() != null)
                hit.GetComponent<Player>().Damage(enemy.facingDir);
        }
    }

    //开启可以被弹反击晕的时间窗口
    protected void OpenCounterWindow() => enemy.OpenCounterAttackWindow();
    //关闭可以被弹反击晕的时间窗口
    protected void CloseCounterWindow() => enemy.CloseCounterAttackWindow();
}
