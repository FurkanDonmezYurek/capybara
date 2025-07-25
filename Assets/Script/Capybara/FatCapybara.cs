
using UnityEngine;
using DG.Tweening;

public class FatCapybara : Capybara
{
    public override CapybaraType Type => CapybaraType.Fat;
    public override float SeatChangeTime => 1.5f;
    private SeatSlot secondSlot;

    public override bool IsMovable()
    {
        if (currentSlot == null) return false;

        int nextIndex = currentSlot.seatIndex + 1;
        var neighborSlot = GridManager.Instance.GetSlot(currentSlot.isLeftSide, currentSlot.rowIndex, nextIndex);

        return currentSlot.isCorridorSide &&
               neighborSlot != null &&
               neighborSlot.IsEmpty();
    }

    public override void MoveTo(SeatSlot targetSlot, float duration = 0.3f)
    {
        int secondIndex = targetSlot.seatIndex + 1;
        var second = GridManager.Instance.GetSlot(targetSlot.isLeftSide, targetSlot.rowIndex, secondIndex);

        if (targetSlot == null || second == null || !targetSlot.IsEmpty() || !second.IsEmpty())
            return;

        currentSlot?.ClearCapybara();
        secondSlot?.ClearCapybara();

        targetSlot.SetCapybara(this);
        second.SetCapybara(this);

        currentSlot = targetSlot;
        secondSlot = second;

        Vector3 center = (targetSlot.transform.position + second.transform.position) / 2f;
        transform.localScale = new Vector3(2f, 1f, 1f);
        transform.DOMove(center, duration).SetEase(Ease.OutQuad);
    }
}
