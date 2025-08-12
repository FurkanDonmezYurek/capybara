using UnityEngine;

[CreateAssetMenu(fileName = "TutorialData", menuName = "Game/Tutorial Data")]
public class TutorialData : ScriptableObject
{
    public TutorialType tutorialType;
    public AnimatorOverrideController animatorController; 
}
