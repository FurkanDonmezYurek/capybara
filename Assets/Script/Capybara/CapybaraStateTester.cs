using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CapybaraStateTester : MonoBehaviour
{
    public Button idleButton;
    public Button normalSitButton;
    public Button fatSitButton;
    public Button childSitButton;
    public Button walkButton;
    public Button runButton;
    public Button sleepButton;
    public Button freezeButton;


    public CapybaraStateMachine stateMachine;

    private void Start()
    {
        stateMachine.FonksiyonStart();
        idleButton.onClick.AddListener(() => SetState(stateMachine.idleState));
        normalSitButton.onClick.AddListener(() => SetState(stateMachine.normalSitState));
        fatSitButton.onClick.AddListener(() => SetState(stateMachine.fatSitState));
        childSitButton.onClick.AddListener(() => SetState(stateMachine.childSitState));
        walkButton.onClick.AddListener(() => SetState(stateMachine.walkState));
        runButton.onClick.AddListener(() => SetState(stateMachine.runState));
        sleepButton.onClick.AddListener(() => SetState(stateMachine.sleepState));
        freezeButton.onClick.AddListener(() => SetState(stateMachine.freezeState));
    }

    private void SetState(CapybaraBaseState newState)
    {
        Debug.Log($"State changed to: {newState.GetType().Name}");
        stateMachine.SetState(newState);
    }
}