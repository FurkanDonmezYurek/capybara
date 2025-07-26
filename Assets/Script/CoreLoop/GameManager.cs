using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private Capybara selectedCapybara;
    private List<SeatGroup> cachedSeatGroups;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        InitializeSeatGroupsCache();
    }

    // TODO: Make these so that there is no  change, instead a little visual effect plays (particle or animation)
    public void OnCapybaraClicked(Capybara capybara)
    {
        if (!capybara.IsMovable())
        {
            Debug.Log("Clicked capybara is not movable!");
            return;
        }

        if (selectedCapybara != null)
        {
            selectedCapybara.GetComponent<Renderer>().material.color =
                selectedCapybara.color;
        }

        selectedCapybara = capybara;
        selectedCapybara.GetComponent<Renderer>().material.color = Color.yellow;
    }

    // Check all groups burdan kaldırıldı. Bu checki seat group bazında capybara.cs den yapıyoruz suan.
    public void OnSeatClicked(Seat seat)
    {
        if (selectedCapybara == null)
            return;

        if (seat.IsEmpty)
        {
            selectedCapybara.SitSeat(seat);
            selectedCapybara.GetComponent<Renderer>().material.color =
                selectedCapybara.color;
            selectedCapybara = null;
        }
    }


    // TODO: Sadece kapibaranın yerini değiştirecegi hedef konumdaki seat grup checklenecek.
    //Optimize edilecek... Not: Cache eklendi eğer yeterince optimizeyse bu yorum satırını kaldırabilirsiniz.
    void CheckAllGroups()
    {
        foreach (var group in cachedSeatGroups)
            group.CheckGroupColor();
    }

    // Cache seat groups for performance
    public void InitializeSeatGroupsCache()
    {
        cachedSeatGroups = new List<SeatGroup>();

        SeatGroup[] allGroups = FindObjectsOfType<SeatGroup>();

        foreach (var group in allGroups)
        {
            cachedSeatGroups.Add(group);
            Debug.Log("Cached group: " + group.name);
        }
    }

    public List<SeatGroup> GetCachedSeatGroups()
    {
        return cachedSeatGroups;
    }

    // DONT FORGET TO USE THIS WHEN RESETTING THE LEVEL!
    public void ClearSeatGroupCache()
    {
        cachedSeatGroups.Clear();
    }

    public Seat GetRandomAvailableSeat()
    {
        List<Seat> availableSeats = GetAvailableSeatsFromCache();
        if (availableSeats == null || availableSeats.Count == 0)
            return null;

        int index = Random.Range(0, availableSeats.Count);
        return availableSeats[index];
    }


    public List<Seat> GetAvailableSeatsFromCache()
    {
        List<Seat> availableSeats = new();

        foreach (var group in cachedSeatGroups)
        {
            foreach (var seat in group.seatsInGroup)
            {
                if (seat.IsEmpty)
                    availableSeats.Add(seat);
            }
        }

        return availableSeats;
    }

    public Seat GetRightNeighborSlot(Seat seat)
    {
        foreach (var group in cachedSeatGroups)
        {
            var index = group.seatsInGroup.IndexOf(seat);
            if (index >= 0 && index < group.seatsInGroup.Count - 1)
            {
                return group.seatsInGroup[index + 1];
            }
        }

        return null;
    }

}
