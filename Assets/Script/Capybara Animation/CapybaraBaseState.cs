using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapybaraBaseState : BaseState
{
    protected CapybaraStateMachine Capybara { get; }

    public CapybaraBaseState(CapybaraStateMachine c)
    {
        Capybara = c;
    }

    public override void Enter()
    {
        throw new System.NotImplementedException();
    }

    public override void Tick()
    {
        throw new System.NotImplementedException();
    }

    public override void Exit()
    {
        throw new System.NotImplementedException();
    }
}
