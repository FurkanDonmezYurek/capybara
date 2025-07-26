using UnityEngine;
using DG.Tweening;

public class FatCapybara : Capybara
{
    public override CapybaraType Type => CapybaraType.Fat;
    public override float SeatChangeTime => 1.5f;

    private Seat secondSlot;

    public override bool IsMovable()
    {
        if (currentSlot == null || isLocked || isFrozen)
            return false;

        Seat neighbor = GameManager.Instance.GetRightNeighborSlot(currentSlot);

        return neighbor != null && currentSlot.IsEmpty == false && neighbor.IsEmpty;
    }
    
    public override void Freeze()
    {
        base.Freeze();

        if (iceCubeVisual != null)
        {
            iceCubeVisual.transform.localScale = new Vector3(2f, 1f, 1f); // Genişlet
            iceCubeVisual.SetActive(true);
        }
    }

    public override void Unfreeze()
    {
        base.Unfreeze();

        if (iceCubeVisual != null)
        {
            iceCubeVisual.transform.localScale = Vector3.one; // Geri eski boyuta getir
            iceCubeVisual.SetActive(false);
        }
    }


    public override void SitSeat(Seat targetSlot)
    {
        if (!IsMovable() || targetSlot == null)
            return;

        Seat right = GameManager.Instance.GetRightNeighborSlot(targetSlot);

        if (right == null || !targetSlot.IsEmpty || !right.IsEmpty)
            return;

        currentSlot?.ClearCapybara();
        secondSlot?.ClearCapybara();

        targetSlot.SetCapybara(this);
        right.SetCapybara(this);

        currentSlot = targetSlot;
        secondSlot = right;

        Vector3 center = (targetSlot.transform.position + right.transform.position) / 2f;
        transform.localScale = new Vector3(2f, 1f, 1f);
        transform.DOMove(center, SeatChangeTime).SetEase(Ease.OutQuad);
    }

    public override void SetLockState(bool state)
    {
        base.SetLockState(state);
        if (state)
        {
            // İkinci slot da kilitlenmiş sayılır
            secondSlot?.SetCapybara(this); // yine bu capybara ile işaretli kalır
        }
    }
}
