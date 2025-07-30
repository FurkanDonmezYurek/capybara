using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapybaraFatSitState : CapybaraBaseState
{
    public CapybaraFatSitState(CapybaraStateMachine c) : base(c)
    {
    }

    public override void Enter()
    {
        Capybara.WAnimation.SetSitAnim(true);
        Capybara.WAnimation.SetMovementAnimByMagnitude(1f, true);
    }

    public override void Tick()
    {

    }

    public override void Exit()
    {
        Capybara.WAnimation.SetMovementAnimByMagnitude(0, true);
        Capybara.WAnimation.SetSitAnim(false);
    }
}
