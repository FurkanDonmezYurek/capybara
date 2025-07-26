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
    }

    // TODO: Make these so that there is no  change, instead a little visual effect plays (particle or animation)
    public void OnCapybaraClicked(Capybara capybara)
    {
        if (!capybara.IsMovable())
        {
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

        CheckAllGroups();
    }


    // TODO: Sadece kapibaranın yerini değiştirecegi hedef konumdaki seat grup checklenecek.
    //Optimize edilecek... Not: Cache eklendi eğer yeterince optimizeyse bu yorum satırını kaldırabilirsiniz.
    void CheckAllGroups()
    {
        foreach (var group in cachedSeatGroups)
            group.CheckGroupColor();
    }

    // Use this from SeatGroup if you want to cache the seat group
    public void CacheSeatGroup(SeatGroup seatGroup)
    {
        if (!cachedSeatGroups.Contains(seatGroup))
            cachedSeatGroups.Add(seatGroup);
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
