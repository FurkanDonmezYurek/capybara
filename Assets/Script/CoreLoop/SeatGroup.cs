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

    public void LockGroup()
    {
        IsGroupLocked = true;
        foreach (var seat in seatsInGroup)
            seat.currentCapybara?.Lock();

        AudioManager.Instance.PlaySFX("Match");
        if (HapticsManager.Instance != null)
            HapticsManager.Instance.PlayLightImpactVibration();

        HapticsManager.Instance.PlayHeavyImpactVibration();

        ParticleManager.Instance.Play(
            ParticleType.Explosion,
            seatsInGroup[1].transform.position + new Vector3(0, 1.5f, 0)
        );
    }

    private void TryUnfreezeVerticalNeighbors()
    {
        foreach (var group in GameManager.Instance.GetCachedSeatGroups())
        {
            if (
                group.groupX == this.groupX
                && (group.groupY == this.groupY - 1 || group.groupY == this.groupY + 1)
            )
            {
                foreach (var seat in group.seatsInGroup)
                {
                    var capy = seat.currentCapybara;
                    if (capy != null && capy.IsFrozen)
                    {
                        capy.Unfreeze();
                        AudioManager.Instance.PlaySFX("Unfreeze");
                        if (HapticsManager.Instance != null)
                            HapticsManager.Instance.PlaySoftImpactVibration();
                    }
                }
            }
        }
    }
}
