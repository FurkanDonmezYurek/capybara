using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapybaraChildSitState : CapybaraBaseState
{
    public CapybaraChildSitState(CapybaraStateMachine c) : base(c)
    {
    }
    public override void Enter()
    {
        Capybara.WAnimation.SetSitAnim(true);
        Capybara.WAnimation.SetMovementAnimByMagnitude(0.5f, true);
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
