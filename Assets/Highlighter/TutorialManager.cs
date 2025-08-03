using UnityEngine;

public enum TutorialStep
{
    Step1_TapToStart,
    Step2_ClickPlayButton
}

public class TutorialManager : MonoBehaviour
{
    public GameObject tapToStartUI; // El işareti ve yazı UI
    public GameObject playButtonTutorialUI; // 2. tutorial için el ve yazı

    public Highlighter.CircleEffect highlightHole; // İlk delik
    public Highlighter.CircleEffect highlightPlayButton; // Buton

    private TutorialStep currentStep;

    void Start()
    {
        currentStep = TutorialStep.Step1_TapToStart;
        ShowStep(currentStep);
    }

    void Update()
    {
        if (currentStep == TutorialStep.Step1_TapToStart && Input.GetMouseButtonDown(0))
        {
            currentStep = TutorialStep.Step2_ClickPlayButton;
            ShowStep(currentStep);
        }
    }

    void ShowStep(TutorialStep step)
    {
        tapToStartUI?.SetActive(step == TutorialStep.Step1_TapToStart);
        playButtonTutorialUI?.SetActive(step == TutorialStep.Step2_ClickPlayButton);

        if (highlightHole != null)
            highlightHole.enabled = (step == TutorialStep.Step1_TapToStart);
        
        if (highlightPlayButton != null)
            highlightPlayButton.enabled = (step == TutorialStep.Step2_ClickPlayButton);
    }
}
