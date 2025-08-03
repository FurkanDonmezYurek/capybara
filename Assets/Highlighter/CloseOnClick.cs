using UnityEngine;
using UnityEngine.EventSystems;

public class CloseOnClick : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        gameObject.SetActive(false); // bu paneli kapat
    }
void Update()
{
    if (Input.GetMouseButtonDown(0)) // veya touch
    {
        gameObject.SetActive(false);
    }
}

}