using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能基类
/// </summary>
public class Skill : MonoBehaviour
{
    [SerializeField] protected float cooldown;
    protected float cooldownTimer;


    protected virtual void Update() 
    {
        cooldownTimer -= Time.deltaTime;
    }

    
    public virtual bool CanUseSkill()
    {
        if(cooldownTimer < 0)
        {
            //可以使用技能
            UseSkill();
            cooldownTimer = cooldown;
            return true;
        }


        Debug.Log("Skill is on cooldown.");
        return false;
    }

    public virtual void UseSkill()
    {
        //具体的技能内容
    }
}
