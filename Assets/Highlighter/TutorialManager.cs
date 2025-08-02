using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public TutorialStep[] steps;
    public Image overlay;
    public Image hand;
    public TextMeshProUGUI tutorialText;

    private int currentStep = 0;

    void Start()
    {
        ShowStep(0);
    }

    void ShowStep(int index)
    {
        if (index >= steps.Length)
        {
            overlay.gameObject.SetActive(false);
            hand.gameObject.SetActive(false);
            tutorialText.gameObject.SetActive(false);
            return;
        }

        TutorialStep step = steps[index];
        tutorialText.text = step.message;
        hand.rectTransform.position = step.targetUI.position + step.handOffset;

        overlay.gameObject.SetActive(true);
        hand.gameObject.SetActive(true);
        tutorialText.gameObject.SetActive(true);

        currentStep = index;
    }

    public void OnUserClickArea(RectTransform clickedArea)
    {
        if (clickedArea == steps[currentStep].targetUI)
        {
            ShowStep(currentStep + 1);
        }
    }
}
