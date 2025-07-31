using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapybaraJumpState : CapybaraBaseState
{
    public CapybaraJumpState(CapybaraStateMachine c) : base(c)
    {
    }
    public override void Enter()
    {
        Capybara.WAnimation.SetJumpAnim(true);
        Capybara.WAnimation.SetMovementAnimByMagnitude(0, true);
    }

    public override void Tick()
    {

    }

    public override void Exit()
    {
        Capybara.WAnimation.SetJumpAnim(false);
    }
}
