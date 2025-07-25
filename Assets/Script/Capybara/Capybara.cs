
using UnityEngine;
using DG.Tweening;
public enum CapybaraType
{
    Normal,
    Fat,
    Child,
    Frozen, // TO-DO Frozen capybara tipi kaldırılacak ve bu mekanik base capibara scriptine entegre edilecek.
    Sleepy
}

public class Capybara : MonoBehaviour
{
    public virtual CapybaraType Type => CapybaraType.Normal;
    public virtual float SeatChangeTime => 1f;
    public string colorTag;
    public SeatSlot currentSlot;
    protected bool isClickable;

    protected virtual void Update()
    {
        SetClickable(IsMovable()); // TO-DO Make sure that this check happens in another place instead of the Update method for optimization reasons!
    }

    public virtual void AssignSlot(SeatSlot slot)
    {
        currentSlot = slot;
        transform.position = slot.transform.position;
    }

    public void SetColor(string color)
    {
        colorTag = color;
        // TO-DO Change to respective capybara color visually here
    }

    public virtual bool IsMovable()
    {
        if (currentSlot == null) return false;
        if (currentSlot.isCorridorSide) return true;

        int frontIndex = currentSlot.isLeftSide ? currentSlot.seatIndex + 1 : currentSlot.seatIndex - 1;
        var frontSlot = GridManager.Instance.GetSlot(currentSlot.isLeftSide, currentSlot.rowIndex, frontIndex);
        return frontSlot != null && frontSlot.IsEmpty();
    }

    // Moves the capybara from one seat to another, add corridor logic here when seats are done
    public virtual void MoveTo(SeatSlot targetSlot)
    {
        if (!IsMovable() || targetSlot == null) return;

        currentSlot.ClearCapybara();
        targetSlot.SetCapybara(this);

        transform.DOMove(targetSlot.transform.position, SeatChangeTime).SetEase(Ease.OutQuad);
    }

    protected virtual void SetClickable(bool clickable)
    {
        isClickable = clickable;
    }

    // TO-DO Connect this to the tap input.
    // Select capybara in capybara move manager, handle the seating logic there after that.
    protected virtual void OnTapped()
    {
        if (!isClickable) return;


        Capybara selectedCapy = CapybaraMoveManager.Instance.GetSelected();
        if (selectedCapy == this)
        {
            SetClickable(false);
        }
        else
        {
            CapybaraMoveManager.Instance.SelectCapybara(this);
        }
    }
}
