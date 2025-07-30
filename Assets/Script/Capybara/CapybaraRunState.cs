using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapybaraRunState : CapybaraBaseState
{
    public CapybaraRunState(CapybaraStateMachine c) : base(c)
    {
    }
    public override void Enter()
    {
        Capybara.WAnimation.SetMovementAnimByMagnitude(1, true);
    }

    public override void Tick()
    {

    }

    public override void Exit()
    {
        Capybara.WAnimation.SetMovementAnimByMagnitude(0,true);
    }
}
