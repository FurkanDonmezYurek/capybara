using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapybaraIdleState : CapybaraBaseState
{
    public CapybaraIdleState(CapybaraStateMachine c) : base(c)
    {
    }

    public override void Enter()
    {
        Capybara.WAnimation.SetMovementAnimByMagnitude(0, true);
    }

    public override void Tick()
    {

    }

    public override void Exit()
    {
        Capybara.WAnimation.SetMovementAnimByMagnitude(0, true);
    }
}
