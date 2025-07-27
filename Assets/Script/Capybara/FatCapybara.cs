using DG.Tweening;
using UnityEngine;

public class FatCapybara : Capybara
{
    public override CapybaraType Type => CapybaraType.Fat;
    public override float SeatChangeTime => 1.5f;

    private Seat secondSlot;

    //TODO: toolda yerleştirirken sorun çıkarıyor (currentslot assign edilmiyor)
    public override void SitSeat(Seat targetSlot)
    {
        if (targetSlot == null)
            return;

        Seat right = GameManager.Instance.GetRightNeighborSlot(targetSlot);

        if (right == null)
            return;

        // Önce varsa eski yerlerden temizle
        currentSlot?.ClearCapybara();
        secondSlot?.ClearCapybara();

        // İki koltuğu da işaretle
        targetSlot.SetCapybara(this);
        right.SetCapybara(this);

        // Konum güncelle
        currentSlot = targetSlot;
        secondSlot = right;

        // Ortasına hareket
        Vector3 center = (targetSlot.transform.position + right.transform.position) / 2f;
        transform.localScale = new Vector3(2f, 1f, 1f); // haha funny

        transform
            .DOMove(center, SeatChangeTime)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                CheckTargetSeatMatch(targetSlot); // eşleşme kontrolü
            });
    }

    public override void Lock()
    {
        base.Lock();
        // İkinci slot da kilitlenmiş sayılır
        secondSlot?.SetCapybara(this); // yine bu capybara ile işaretli kalır
    }
}
