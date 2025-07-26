using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private Capybara selectedCapybara;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void OnCapybaraClicked(Capybara capybara)
    {
        if (selectedCapybara != null)
        {
            selectedCapybara.GetComponent<Renderer>().material.color =
                selectedCapybara.capybaraColor;
        }

        selectedCapybara = capybara;
        selectedCapybara.GetComponent<Renderer>().material.color = Color.yellow;
    }

    public void OnSeatClicked(Seat seat)
    {
        if (selectedCapybara == null)
            return;

        if (seat.IsEmpty)
        {
            selectedCapybara.SitOnSeat(seat);
            selectedCapybara.GetComponent<Renderer>().material.color =
                selectedCapybara.capybaraColor;
            selectedCapybara = null;
        }

        CheckAllGroups();
    }

    //Optimize edilecek...
    void CheckAllGroups()
    {
        foreach (var group in FindObjectsOfType<SeatGroup>())
            group.CheckGroupColor();
    }
}
