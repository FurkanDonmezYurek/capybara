using UnityEngine;

public class Seat : MonoBehaviour
{
    public Vector2Int gridPosition;
    public Capybara currentCapybara;
    public bool IsEmpty => currentCapybara == null;

    // Sets current capybara
    public void SetCapybara(Capybara capy)
    {
        currentCapybara = capy;
    }

    public void ClearCapybara()
    {
        currentCapybara = null;
    }

    private void OnMouseDown()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnSeatClicked(this);
    }
}
