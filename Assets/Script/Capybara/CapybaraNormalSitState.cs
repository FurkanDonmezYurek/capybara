using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapybaraNormalSitState : CapybaraBaseState
{
    public CapybaraNormalSitState(CapybaraStateMachine c) : base(c)
    {
    }
    public override void Enter()
    {
        Capybara.WAnimation.SetSitAnim(true);
        Capybara.WAnimation.SetMovementAnimByMagnitude(0, true);
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
