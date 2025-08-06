using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FatCapybara : Capybara
{
    public override CapybaraType Type => CapybaraType.Fat;
    public override float MoveSpeed => 3f;

    public Seat secondSlot;

    // Oyun başlangıcında direkt seat'e yerleştirmek için
    public override void SetSeat(Seat targetSlot)
    {

        if (targetSlot == null)
        {
            return;
        }

        var group = targetSlot.groupOfSeat;
        var groupSeats = group.seatsInGroup;
        int targetIndex = groupSeats.IndexOf(targetSlot);

        if (targetIndex == -1)
        {
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
                primary = leftSeat;
                secondary = targetSlot;
            }
        }

        if (primary == null || secondary == null)
        {
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
        // transform.localScale = new Vector3(2f, 1f, 1f);

        SitAnimation();
    }

    public override void SitSeat(Seat targetSlot)
    {
        if (targetSlot == null)
        {
            return;
        }

        // Eğer current seat yoksa direkt set et
        if (currentSlot == null)
        {
            SetSeat(targetSlot);
            return;
        }
        // Eğer farklı gruplardaysa corridor üzerinden git
        if (!IsSameGroup(currentSlot, targetSlot))
        {
            AnimateCorridorMove(targetSlot);
            return;
        }

        AnimateDirectMove(targetSlot);

        var group = targetSlot.groupOfSeat;
        var groupSeats = group.seatsInGroup;
        int targetIndex = groupSeats.IndexOf(targetSlot);


        if (targetIndex == -1)
        {
            return;
        }

        // Mevcut slotları temizle ÖNCE
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
                    primary = groupSeats[i];
                    secondary = groupSeats[i + 1];
                    break;
                }
            }
        }

        if (primary == null || secondary == null)
        {
            return;
        }

        // Seat'leri işgal et
        try
        {
            primary.SetCapybara(this);
            secondary.SetCapybara(this);

            currentSlot = primary;
            secondSlot = secondary;
        }
        catch (System.Exception e)
        {
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

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            transform.position = center;
            return;
        }
#endif

        transform
            .DOMove(center, duration)
            .OnComplete(() =>
            {
                CheckTargetSeatMatch(primary);
                SitAnimation();
            });
    }

    protected override void AnimateCorridorMove(Seat targetSlot)
    {
        if (currentSlot == null || targetSlot == null)
        {
            return;
        }

        GridSystem gridSystem = FindObjectOfType<GridSystem>();
        if (gridSystem == null)
        {
            return;
        }

        WalkAnimation();

        SeatGroup fromGroup = currentSlot.groupOfSeat;
        SeatGroup toGroup = targetSlot.groupOfSeat;

        int fromX = fromGroup.groupX;
        int toX = toGroup.groupX;
        int y = fromGroup.groupY; // corridor row

        Vector3 startCenter = (currentSlot.transform.position + secondSlot.transform.position) / 2f;
        Vector3 endCenter;

        // Önce uygun hedef çiftini bul
        var groupSeats = toGroup.seatsInGroup;
        int targetIndex = groupSeats.IndexOf(targetSlot);
        Seat primary = null,
            secondary = null;

        // Öncelik: target + sağ
        if (
            targetIndex < groupSeats.Count - 1
            && groupSeats[targetIndex].IsEmpty
            && groupSeats[targetIndex + 1].IsEmpty
        )
        {
            primary = groupSeats[targetIndex];
            secondary = groupSeats[targetIndex + 1];
        }
        // Alternatif: sol + target
        else if (
            targetIndex > 0
            && groupSeats[targetIndex - 1].IsEmpty
            && groupSeats[targetIndex].IsEmpty
        )
        {
            primary = groupSeats[targetIndex - 1];
            secondary = groupSeats[targetIndex];
        }
        // Alternatif: ilk boş çift
        else
        {
            for (int i = 0; i < groupSeats.Count - 1; i++)
            {
                if (groupSeats[i].IsEmpty && groupSeats[i + 1].IsEmpty)
                {
                    primary = groupSeats[i];
                    secondary = groupSeats[i + 1];
                    break;
                }
            }
        }

        if (primary == null || secondary == null)
        {
            return;
        }

        endCenter = (primary.transform.position + secondary.transform.position) / 2f;

        // Slot'ları geçici olarak rezerve et (animasyon sırasında çakışma olmasın)
        currentSlot.ClearCapybara();
        secondSlot.ClearCapybara();

        //transform.localScale = new Vector3(2f, 1f, 1f);

        // PathPoints
        Vector3 fromExit = GetCorridorExitPoint(currentSlot, targetSlot);
        Vector3 toEntry = GetCorridorExitPoint(primary, currentSlot);

        List<Vector3> pathPoints = new();
        pathPoints.Add(startCenter); // A - Başlangıç
        pathPoints.Add(fromExit); // B - Corridor çıkışı

        if (fromX < toX)
        {
            for (int x = fromX; x < toX; x++)
            {
                pathPoints.Add(gridSystem.pathPointsGrid[x.ToString()][y.ToString()]);
            }
        }
        else if (fromX > toX)
        {
            for (int x = fromX - 1; x >= toX; x--)
            {
                pathPoints.Add(gridSystem.pathPointsGrid[x.ToString()][y.ToString()]);
            }
        }

        pathPoints.Add(toEntry); // C - Corridor girişi
        pathPoints.Add(endCenter); // D - Hedef

        // Animasyon
        Sequence seq = DOTween.Sequence();
        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            Vector3 from = pathPoints[i];
            Vector3 to = pathPoints[i + 1];

            float dist = Vector3.Distance(from, to);
            float dur = dist / MoveSpeed;

            seq.AppendCallback(() => LookTowards(to)); // Her adım öncesi bakış yönünü ayarla
            seq.Append(transform.DOMove(to, dur).SetEase(Ease.Linear));
        }

        seq.OnComplete(() =>
        {
            // Hedef seat'leri set et
            try
            {
                primary.SetCapybara(this);
                secondary.SetCapybara(this);
                currentSlot = primary;
                secondSlot = secondary;
            }
            catch (System.Exception e)
            {
                primary?.ClearCapybara();
                secondary?.ClearCapybara();
                currentSlot = null;
                secondSlot = null;
            }

            SitAnimation();

            // Yerleşimi kontrol et
            CheckTargetSeatMatch(primary);
        });
    }

    protected override void AnimateDirectMove(Seat targetSlot)
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

        RunAnimation();

        var group = targetSlot.groupOfSeat;
        var groupSeats = group.seatsInGroup;
        int targetIndex = groupSeats.IndexOf(targetSlot);

        Seat primary = null,
            secondary = null;

        // target + sağ
        if (targetIndex < groupSeats.Count - 1)
        {
            var right = groupSeats[targetIndex + 1];
            if (targetSlot.IsEmpty && right.IsEmpty)
            {
                primary = targetSlot;
                secondary = right;
            }
        }

        // sol + target
        if (primary == null && targetIndex > 0)
        {
            var left = groupSeats[targetIndex - 1];
            if (left.IsEmpty && targetSlot.IsEmpty)
            {
                primary = left;
                secondary = targetSlot;
            }
        }

        if (primary == null || secondary == null)
        {
            return;
        }

        currentSlot = primary;
        secondSlot = secondary;
        primary.SetCapybara(this);
        secondary.SetCapybara(this);

        Vector3 start = transform.position;
        Vector3 end = (primary.transform.position + secondary.transform.position) / 2f;
        float duration = Vector3.Distance(start, end) / MoveSpeed;
        LookTowards(end);

        transform
            .DOMove(end, duration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                CheckTargetSeatMatch(primary);
                SitAnimation();
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
