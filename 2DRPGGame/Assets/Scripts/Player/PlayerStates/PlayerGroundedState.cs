using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        //点击Q键，进入振刀（反击）状态
        if(Input.GetKeyDown(KeyCode.Q))
            stateMachine.ChangeState(player.counterAttack);

        //鼠标左键点击，进入攻击状态
        if(Input.GetKey(KeyCode.Mouse0))
            stateMachine.ChangeState(player.primaryAttack);

        //检测不到地面，进入空中状态
        if(!player.groundDetected)
            stateMachine.ChangeState(player.airState);

        //点击跳跃键，进入跳跃状态
        if(Input.GetKeyDown(KeyCode.Space) && player.groundDetected)
            stateMachine.ChangeState(player.jumpState);
    }
}
