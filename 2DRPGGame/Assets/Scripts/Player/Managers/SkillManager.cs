using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : SingletonMono<SkillManager>
{
    public Dash_Skill dash { get; private set; }

    void Start()
    {
        dash = GetComponent<Dash_Skill>();
    }
}
