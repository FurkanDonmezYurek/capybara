using DG.Tweening;
using UnityEngine;

public enum CapybaraType
{
    Normal,
    Fat,
    Child,
    Sleepy,
}

public class Capybara : MonoBehaviour
{
    public virtual CapybaraType Type => CapybaraType.Normal;
    public virtual float SeatChangeTime => 1f;
    public Color color;
    public Seat currentSlot;
    protected bool isLocked;
    protected bool isFrozen;
    public bool IsFrozen => isFrozen;
    public GameObject iceCubeVisual; // Assigned in prefab or instantiated

    public virtual void Start()
    {
        // 1/10 Chance to be frozen for testing purposes.
        if (Random.Range(0, 10) == 0)
        {
            Freeze();
        }
    }

    public virtual void SetSeat(Seat slot)
    {
        currentSlot = slot;
        transform.position = slot.transform.position;
    }

    public void SetColor(Color color)
    {
        this.color = color;
        GetComponent<Renderer>().material.color = color;
    }

    public virtual void Freeze()
    {
        isFrozen = true;
        if (iceCubeVisual != null)
            iceCubeVisual.SetActive(true);
    }

    public virtual void Unfreeze()
    {
        isFrozen = false;
        if (iceCubeVisual != null)
            iceCubeVisual.SetActive(false);
    }

    public virtual bool IsMovable()
    {
        return !isLocked && !isFrozen;
    }

    public virtual void SitSeat(Seat targetSlot)
    {
        if (!IsMovable() || targetSlot == null)
            return;

        currentSlot?.ClearCapybara();
        targetSlot.SetCapybara(this);
        currentSlot = targetSlot;

        transform
            .DOMove(targetSlot.transform.position, SeatChangeTime)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                CheckTargetSeatMatch(targetSlot);
            });
    }

    public void CheckTargetSeatMatch(Seat targetSlot)
    {
        // Seat'in bağlı olduğu group'u bul
        var group = targetSlot.GetComponentInParent<SeatGroup>();
        group?.CheckGroupColor();
    }

    public virtual void Lock()
    {
        Debug.Log("Locked seat: " + name);
        isLocked = true;
    }

    protected virtual void OnMouseDown()
    {
        if (isLocked || isFrozen)
            return;

        GameManager.Instance.OnCapybaraClicked(this);
    }
}
