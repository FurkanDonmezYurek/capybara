using DG.Tweening;
using UnityEngine;

public class VehicleToggle : MonoBehaviour
{
    public void Show()
    {
        gameObject.SetActive(true);
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }

    public void Hide()
    {
        transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack)
            .OnComplete(() => gameObject.SetActive(false));
    }

    public void Toggle()
    {
        if (!gameObject.activeSelf)
            Show();
        else
            Hide();
    }
}
