using UnityEngine;

public class Seat : MonoBehaviour
{
    public Vector2Int gridPosition;
    public Capybara currentCapybara;
    public bool IsEmpty => currentCapybara == null;
    public bool isCorridorSide;
    public SeatGroup groupOfSeat;

    // Sets current capybara
    public void SetCapybara(Capybara capy)
    {
        currentCapybara = capy;
    }

    public void ClearCapybara()
    {
        currentCapybara = null;
    }

    public bool IsOccupiedBy(Capybara capy)
    {
        return currentCapybara == capy;
    }

    private void OnMouseDown()
    {
        if (currentCapybara == null)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnSeatClicked(this);
        }
        else
        {
            currentCapybara.ClickedCapybara();
        }
    }
}
