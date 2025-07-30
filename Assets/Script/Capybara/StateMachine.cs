using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StateMachine
{
    [TextArea(2, 3)][SerializeField] private string stateName;


    // Holds current state 
    private BaseState _currentState;


    // All transitions
    private readonly Dictionary<BaseState, List<Transition>> _allTransitions =
        new Dictionary<BaseState, List<Transition>>();

    // Current transitions of a spesific state
    private List<Transition> _currentTransitions = new List<Transition>();

    // Transitions that can be transitioned in any case like dying
    private readonly List<Transition> _anyTransitions = new List<Transition>();

    // For memory-allocating issues
    private readonly List<Transition> _emptyTransitions = new List<Transition>(0);


    // Checks for transitions
    // If transition condition met, change the state and call its update func..
    public void Tick()
    {
        Transition transition = GetTransition();

        if (transition != null) SetState(transition.To);

        _currentState?.Tick();
    }

    public void SetState(BaseState nextState)
    {
        if (nextState == _currentState) return;

        _currentState?.Exit();
        _currentState = nextState;

        // If current state's transition is empty, just set the current transition to empty.
        if (!_allTransitions.TryGetValue(_currentState, out _currentTransitions))
            _currentTransitions = _emptyTransitions;
        _currentState?.Enter();

        if (_currentState != null) stateName = _currentState.ToString();
    }

    public void AddTransition(BaseState from, BaseState to, Func<bool> predicate)
    {
        // If states value is empty, create list & set to it.

        if (!_allTransitions.TryGetValue(from, out var transitions))
        {
            transitions = new List<Transition>();
            _allTransitions[from] = transitions;
        }

        // Add transition to the 
        transitions.Add(new Transition(to, predicate));
    }

    public void AddAnyTransition(BaseState to, Func<bool> predicate)
    {
        _anyTransitions.Add(new Transition(to, predicate));
    }

    public bool IsInState(BaseState state)
    {
        return _currentState == state;
    }

    private Transition GetTransition()
    {
        foreach (var transition in _anyTransitions)
        {
            if (transition.Condition()) return transition;
        }

        foreach (var transition in _currentTransitions)
        {
            if (transition.Condition()) return transition;
        }

        return null;
    }

    private class Transition
    {
        public Func<bool> Condition { get; }
        public BaseState To { get; }

        public Transition(BaseState to, Func<bool> condition)
        {
            To = to;
            Condition = condition;
        }
    }
}