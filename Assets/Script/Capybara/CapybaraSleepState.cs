using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapybaraSleepState : CapybaraBaseState
{
    public CapybaraSleepState(CapybaraStateMachine c) : base(c)
    {
    }
    public override void Enter()
    {
        Capybara.WAnimation.SetSleepAnim(true);
        Capybara.WAnimation.SetMovementAnimByMagnitude(0, true);
    }

    public override void Tick()
    {

    }

    public override void Exit()
    {
        Capybara.WAnimation.SetSleepAnim(false);
    }
}
