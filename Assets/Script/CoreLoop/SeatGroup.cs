using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SeatGroup : MonoBehaviour
{
    public List<Seat> seatsInGroup;
    public int groupY; // row index
    public int groupX; // column index
    public bool IsGroupLocked { get; private set; }

    public void CheckGroupColor()
    {
        Debug.Log("Checking group color for SeatGroup: " + name);

        if (IsGroupLocked)
            return;

        if (seatsInGroup.Any(s => s.currentCapybara == null))
            return;

        if (seatsInGroup.Any(s => s.currentCapybara.IsFrozen))
            return;

        var distinctColors = seatsInGroup.Select(s => s.currentCapybara.color).Distinct();
        if (distinctColors.Count() == 1)
        {
            LockGroup();
            TryUnfreezeVerticalNeighbors();
        }
    }

    void LockGroup()
    {
        Debug.Log("Locking seats in group...");

        IsGroupLocked = true;

        foreach (var seat in seatsInGroup)
            seat.currentCapybara?.Lock();

        //koltuk kilitlendi efekti atacaz...
    }

    // Bu metot, dikey komşu gruplardaki kapybaraları serbest bırakır
    private void TryUnfreezeVerticalNeighbors()
    {
        foreach (var group in GameManager.Instance.GetCachedSeatGroups())
        {
            // Aynı sütunda olacak ve satırı ya +1 ya -1 olacak
            if (
                group.groupX == this.groupX
                && (group.groupY == this.groupY - 1 || group.groupY == this.groupY + 1)
            )
            {
                foreach (var seat in group.seatsInGroup)
                {
                    var capy = seat.currentCapybara;
                    Debug.Log("Capybara name: " + (capy != null ? capy.name : "null"));
                    Debug.Log("IsFrozen: " + (capy != null ? capy.IsFrozen : "null"));
                    if (capy != null && capy.IsFrozen)
                    {
                        capy.Unfreeze();
                    }
                }
            }
        }
    }
}
