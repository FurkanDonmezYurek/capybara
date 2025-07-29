using DG.Tweening;
using UnityEngine;

public class FatCapybara : Capybara
{
    // FIXME: CORRIDOR MECHANIC DOES NOT WORK FOR THE FAT CAPYBARA RIGHT NOW,
    public override CapybaraType Type => CapybaraType.Fat;
    public override float MoveSpeed => 1.5f;

    public Seat secondSlot;

    // Oyun başlangıcında direkt seat'e yerleştirmek için
    public override void SetSeat(Seat targetSlot)
    {
        Debug.Log("Setting fat capybara to seat directly...");

        if (targetSlot == null)
        {
            Debug.Log("Target slot is null!");
            return;
        }

        var group = targetSlot.groupOfSeat;
        var groupSeats = group.seatsInGroup;
        int targetIndex = groupSeats.IndexOf(targetSlot);

        if (targetIndex == -1)
        {
            Debug.Log("Target slot not found in its group!");
            return;
        }

        // Uygun seat çiftini bul
        Seat primary = null;
        Seat secondary = null;

        // 1. Öncelik: target + sağdaki seat
        if (targetIndex < groupSeats.Count - 1)
        {
            Seat rightSeat = groupSeats[targetIndex + 1];
            if (targetSlot.IsEmpty && rightSeat.IsEmpty)
            {
                Debug.Log("Direct set - Pair found: TARGET + RIGHT");
                primary = targetSlot;
                secondary = rightSeat;
            }
        }

        // 2. Alternatif: soldaki + target
        if (primary == null && targetIndex > 0)
        {
            Seat leftSeat = groupSeats[targetIndex - 1];
            if (leftSeat.IsEmpty && targetSlot.IsEmpty)
            {
                Debug.Log("Direct set - Pair found: LEFT + TARGET");
                primary = leftSeat;
                secondary = targetSlot;
            }
        }

        if (primary == null || secondary == null)
        {
            Debug.LogWarning("FatCapybara: No valid adjacent seat pair found for direct setting.");
            return;
        }

        // Seat'leri işgal et
        primary.SetCapybara(this);
        secondary.SetCapybara(this);

        currentSlot = primary;
        secondSlot = secondary;

        // Pozisyonu center'a direkt set et
        Vector3 center = (primary.transform.position + secondary.transform.position) / 2f;
        transform.position = center;
        transform.localScale = new Vector3(2f, 1f, 1f);

        Debug.Log(
            $"Fat capybara directly set to seats: Primary={primary.name}, Secondary={secondary.name}"
        );
    }

    public override void SitSeat(Seat targetSlot)
    {
        Debug.Log("Attempting to sit fat capybara...");

        if (targetSlot == null)
        {
            Debug.Log("Target slot is null!");
            return;
        }

        // Eğer current seat yoksa direkt set et
        if (currentSlot == null)
        {
            Debug.Log("No current seat, using direct set...");
            SetSeat(targetSlot);
            return;
        }

        var group = targetSlot.groupOfSeat;
        var groupSeats = group.seatsInGroup;
        int targetIndex = groupSeats.IndexOf(targetSlot);

        Debug.Log($"Target slot index in group: {targetIndex}");

        if (targetIndex == -1)
        {
            Debug.Log("Target slot not found in its group!");
            return;
        }

        // Mevcut slotları temizle ÖNCE
        Debug.Log("Clearing previous slots if any...");
        if (currentSlot != null)
        {
            currentSlot.ClearCapybara();
            currentSlot = null;
        }
        if (secondSlot != null)
        {
            secondSlot.ClearCapybara();
            secondSlot = null;
        }

        // Uygun seat çiftini bul
        Seat primary = null;
        Seat secondary = null;

        // 1. Öncelik: target + sağdaki seat
        if (targetIndex < groupSeats.Count - 1)
        {
            Seat rightSeat = groupSeats[targetIndex + 1];
            if (targetSlot.IsEmpty && rightSeat.IsEmpty)
            {
                Debug.Log("Pair found: TARGET + RIGHT");
                primary = targetSlot;
                secondary = rightSeat;
            }
        }

        // 2. Alternatif: soldaki + target
        if (primary == null && targetIndex > 0)
        {
            Seat leftSeat = groupSeats[targetIndex - 1];
            if (leftSeat.IsEmpty && targetSlot.IsEmpty)
            {
                Debug.Log("Pair found: LEFT + TARGET");
                primary = leftSeat;
                secondary = targetSlot;
            }
        }

        // 3. Son alternatif: target'ın her iki yanındaki boş slotları kontrol et
        if (primary == null)
        {
            for (int i = 0; i < groupSeats.Count - 1; i++)
            {
                if (groupSeats[i].IsEmpty && groupSeats[i + 1].IsEmpty)
                {
                    Debug.Log($"Alternative pair found at indices {i} and {i + 1}");
                    primary = groupSeats[i];
                    secondary = groupSeats[i + 1];
                    break;
                }
            }
        }

        if (primary == null || secondary == null)
        {
            Debug.LogWarning("FatCapybara: No valid adjacent seat pair found in the group.");
            return;
        }

        // Seat'leri işgal et
        try
        {
            primary.SetCapybara(this);
            secondary.SetCapybara(this);

            currentSlot = primary;
            secondSlot = secondary;

            Debug.Log(
                $"Successfully assigned seats: Primary={primary.gridPosition}, Secondary={secondary.gridPosition}"
            );
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error setting capybara to seats: {e.Message}");
            // Hata durumunda temizle
            primary?.ClearCapybara();
            secondary?.ClearCapybara();
            currentSlot = null;
            secondSlot = null;
            return;
        }

        // Hareket hesapla ve gerçekleştir
        Vector3 center = (primary.transform.position + secondary.transform.position) / 2f;
        float distance = Vector3.Distance(transform.position, center);
        float duration = distance / MoveSpeed;

        // Scale ayarla
        transform.localScale = new Vector3(2f, 1f, 1f);

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            Debug.Log("Editor mode: setting position instantly.");
            transform.position = center;
            return;
        }
#endif

        Debug.Log($"Moving capybara to center position: {center}");
        transform
            .DOMove(center, duration)
            .OnComplete(() =>
            {
                Debug.Log("Movement complete, checking match...");
                CheckTargetSeatMatch(primary);
            });
    }

    public override void Lock()
    {
        base.Lock();
        // İkinci slot da kilitlenmiş sayılır
        if (secondSlot != null)
        {
            secondSlot.SetCapybara(this);
        }
    }

    // Capybara'yı tamamen temizlemek için
    public void ClearFromSeats()
    {
        if (currentSlot != null)
        {
            currentSlot.ClearCapybara();
            currentSlot = null;
        }
        if (secondSlot != null)
        {
            secondSlot.ClearCapybara();
            secondSlot = null;
        }
    }

    // Debug için
    private void OnDestroy()
    {
        ClearFromSeats();
    }
}
