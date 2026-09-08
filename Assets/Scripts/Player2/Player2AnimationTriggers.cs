using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2AnimationTriggers : MonoBehaviour
{
    private Player2 player;

    void Awake()
    {
        player = GetComponentInParent<Player2>();
    }

    private void CurrentStateTrigger()
    {
        player.CallAnimationTrigger();
    }

    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius[player.attackNum]);

        foreach(var hit in colliders)
        {
            if(hit.GetComponent<Enemy>() != null)
                hit.GetComponent<Enemy>().Damage(player.facingDir);
        }
    }
}
