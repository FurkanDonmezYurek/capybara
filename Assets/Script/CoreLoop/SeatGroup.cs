using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SeatGroup : MonoBehaviour
{
    public List<Seat> seatsInGroup;
    public bool IsGroupLocked { get; private set; }

    public void CheckGroupColor()
    {
        if (IsGroupLocked)
            return;

        if (seatsInGroup.Any(s => s.currentCapybara == null))
            return;

        var distinctColors = seatsInGroup.Select(s => s.currentCapybara.capybaraColor).Distinct();

        if (distinctColors.Count() == 1)
        {
            LockGroup();
        }
    }

    void LockGroup()
    {
        IsGroupLocked = true;

        foreach (var seat in seatsInGroup)
            seat.currentCapybara?.Lock();

        //koltuk kilitlendi efekti atacaz...
    }
}
