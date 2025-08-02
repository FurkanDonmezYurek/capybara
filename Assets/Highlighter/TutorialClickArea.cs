using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialClickArea : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        FindObjectOfType<TutorialManager>().OnUserClickArea(transform as RectTransform);
    }
}
