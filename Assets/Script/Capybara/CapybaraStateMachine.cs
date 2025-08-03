using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapybaraStateMachine : MonoBehaviour
{
    public CapybaraIdleState idleState { get; set; }
    public CapybaraNormalSitState normalSitState { get; set; }
    public CapybaraFatSitState fatSitState { get; set; }
    public CapybaraChildSitState childSitState { get; set; }
    public CapybaraWalkState walkState { get; set; }
    public CapybaraRunState runState { get; set; }
    public CapybaraSleepState sleepState { get; set; }
    public CapybaraFreezeState freezeState { get; set; }
    public CapybaraJumpState jumpState { get; set; }

    /// <summary>
    /// Machine reference
    /// </summary>
    [field: SerializeField]
    protected StateMachine Machine { get; private set; }
    [field: SerializeField]
    public CapybaraAnimation WAnimation { get; set; }
    [field: SerializeField]
    public Animator _animator;
    public void FonksiyonStart()
    {
        WAnimation = new CapybaraAnimation(_animator);
        // Set Machine
        Machine = new StateMachine();
        // Create states
        CreateStates();

    }
    public void CreateStates()
    {
        idleState = new CapybaraIdleState(this);
        normalSitState = new CapybaraNormalSitState(this);
        fatSitState = new CapybaraFatSitState(this);
        childSitState = new CapybaraChildSitState(this);
        walkState = new CapybaraWalkState(this);
        runState = new CapybaraRunState(this);
        sleepState = new CapybaraSleepState(this);
        freezeState = new CapybaraFreezeState(this);
        jumpState = new CapybaraJumpState(this);
    }
    public void SetState(CapybaraBaseState state)
    {
        Machine.SetState(state);
    }
}
