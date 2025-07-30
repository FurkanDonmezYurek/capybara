using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapybaraWalkState : CapybaraBaseState
{
    public CapybaraWalkState(CapybaraStateMachine c) : base(c)
    {
    }
    public override void Enter()
    {
        Capybara.WAnimation.SetMovementAnimByMagnitude(0.5f, true);
    }

    public override void Tick()
    {

    }

    public override void Exit()
    {
        Capybara.WAnimation.SetMovementAnimByMagnitude(0, true);
    }
}
