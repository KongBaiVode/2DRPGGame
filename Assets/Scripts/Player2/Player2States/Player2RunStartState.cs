using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2RunStartState : Player2GroundedState
{
    public Player2RunStartState(Player2 _player, Player2StateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.HandleFlip(xInput);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        player.SetVelocity(xInput * player.moveSpeed, rb.velocity.y);

        if(xInput == 0)
            stateMachine.ChangeState(player.runStopState);

        if(triggerCalled)
            stateMachine.ChangeState(player.runState);
    }
}
