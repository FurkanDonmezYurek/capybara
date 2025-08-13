using TMPro;
using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(fileName = "TutorialData", menuName = "CapybaraSort/Tutorial Data")]
public class TutorialData : ScriptableObject
{
    public TutorialType tutorialType;
    public AnimatorController animatorController; 
}
