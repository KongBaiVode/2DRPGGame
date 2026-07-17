using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : SingletonMono<SkillManager>
{
    #region Skills
    public Dash_Skill dash { get; private set; }
    public Clone_Skill clone { get; private set; }

    #endregion

    void Start()
    {
        dash = GetComponent<Dash_Skill>();
        clone = GetComponent<Clone_Skill>();
    }
}
