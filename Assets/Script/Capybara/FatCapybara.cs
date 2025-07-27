using UnityEngine;
using DG.Tweening;

public class FatCapybara : Capybara
{
    public override CapybaraType Type => CapybaraType.Fat;
    public override float SeatChangeTime => 1.5f;

    private Seat secondSlot;

    //TODO: toolda yerleştirirken sorun çıkarıyor (currentslot assign edilmiyor)
    public override void SitSeat(Seat targetSlot)
    {
        Debug.Log("Attempting to sit fat capybara...");

        if (targetSlot == null)
        {
            Debug.Log("Target slot is null!");
            return;
        }

        var group = targetSlot.groupOfSeat;
        var groupSeats = group.seatsInGroup;
        int index = groupSeats.IndexOf(targetSlot);

        Debug.Log($"Target slot index in group: {index}");

        if (index == -1)
        {
            Debug.Log("Target slot not found in its group!");
            return;
        }

        Seat left = (index > 0) ? groupSeats[index - 1] : null;
        Seat right = (index < groupSeats.Count - 1) ? groupSeats[index + 1] : null;

        Debug.Log($"Left seat: {(left == null ? "null" : left.name)} | Right seat: {(right == null ? "null" : right.name)}");
        Debug.Log($"target.IsEmpty={targetSlot.IsEmpty}, left.IsEmpty={(left != null ? left.IsEmpty : false)}, right.IsEmpty={(right != null ? right.IsEmpty : false)}");

        Seat primary = null;
        Seat secondary = null;

        // Öncelik: target + right boşsa
        if (targetSlot.IsEmpty && right != null && right.IsEmpty)
        {
            Debug.Log("Pair found: TARGET + RIGHT");
            primary = targetSlot;
            secondary = right;
        }
        // Değilse: left + target boşsa
        else if (left != null && left.IsEmpty && targetSlot.IsEmpty)
        {
            Debug.Log("Pair found: LEFT + TARGET");
            primary = left;
            secondary = targetSlot;
        }
        else
        {
            Debug.LogWarning("FatCapybara: No valid adjacent seat pair found.");
            return;
        }

        // Eski slotları temizle
        Debug.Log("Clearing previous slots if any...");
        currentSlot?.ClearCapybara();
        secondSlot?.ClearCapybara();

        // Yeni slotları ata
        primary.SetCapybara(this);
        secondary.SetCapybara(this);

        currentSlot = primary;
        secondSlot = secondary;

        Vector3 center = (primary.transform.position + secondary.transform.position) / 2f;
        transform.localScale = new Vector3(2f, 1f, 1f);

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            Debug.Log("Editor mode: setting position instantly.");
            transform.position = center;
            return;
        }
#endif

        Debug.Log("Moving capybara to center of selected pair...");
        transform
            .DOMove(center, SeatChangeTime)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                Debug.Log("Movement complete, checking match...");
                CheckTargetSeatMatch(primary); // sol koltuk üzerinden match check
            });
    }



    public override void Lock()
    {
        base.Lock();
        // İkinci slot da kilitlenmiş sayılır
        secondSlot?.SetCapybara(this); // yine bu capybara ile işaretli kalır
    }
}
