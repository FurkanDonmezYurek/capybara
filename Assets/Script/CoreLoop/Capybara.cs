using UnityEngine;

public class Capybara : MonoBehaviour
{
    public Color capybaraColor;
    public Seat currentSeat;
    private bool isLocked;

    private void OnMouseDown()
    {
        if (isLocked)
            return;
        GameManager.Instance.OnCapybaraClicked(this);
    }

    public void SitOnSeat(Seat seat)
    {
        if (currentSeat != null)
            currentSeat.currentCapybara = null;

        currentSeat = seat;
        currentSeat.currentCapybara = this;

        //hareket muhabbetini burda yapacaz(değişecek)...
        transform.position = seat.transform.position;
    }

    public void Lock()
    {
        isLocked = true;

        //kilitlendigini göster...
    }
}
