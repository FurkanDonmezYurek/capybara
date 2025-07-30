using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapybaraFreezeState : CapybaraBaseState
{
    public CapybaraFreezeState(CapybaraStateMachine c) : base(c)
    {
    }
    public override void Enter()
    {
        Capybara.WAnimation.SetFreezeAnim(true);
        Capybara.WAnimation.SetMovementAnimByMagnitude(0, true);
    }

    public override void Tick()
    {

    }

    public override void Exit()
    {
        Capybara.WAnimation.SetFreezeAnim(false);
    }
}