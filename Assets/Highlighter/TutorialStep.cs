using UnityEngine;

[CreateAssetMenu(menuName = "Tutorial/Tutorial Step")]
public class TutorialStep : ScriptableObject
{
    public string message;
    public RectTransform targetUI;
    public Vector3 handOffset;
}
