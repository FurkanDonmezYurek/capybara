using System.Linq;
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
    public virtual float MoveSpeed => 2f;
    public Color color;
    public Seat currentSlot;
    protected bool isLocked;
    protected bool isFrozen;
    public bool IsFrozen => isFrozen;
    public GameObject iceCubeVisual; // Assigned in prefab or instantiated

    public virtual void Start()
    {
        // Empty
    }

    public virtual void SetSeat(Seat slot)
    {
        currentSlot = slot;
        slot.SetCapybara(this);
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
        if (!CanSitTo(targetSlot))
            return;

        if (currentSlot == null)
        {
            DirectPlace(targetSlot);
            return;
        }

        if (IsSameGroup(currentSlot, targetSlot))
        {
            AnimateDirectMove(targetSlot);
        }
        else
        {
            AnimateCorridorMove(targetSlot);
        }
    }

    protected virtual bool CanSitTo(Seat targetSlot)
    {
        return IsMovable() && targetSlot != null;
    }

    protected virtual void DirectPlace(Seat slot)
    {
        slot.SetCapybara(this);
        currentSlot = slot;
        transform.position = slot.transform.position;
    }

    protected virtual bool IsSameGroup(Seat a, Seat b)
    {
        return a.groupOfSeat == b.groupOfSeat;
    }

    protected virtual void AnimateDirectMove(Seat targetSlot)
    {
        currentSlot.ClearCapybara();
        targetSlot.SetCapybara(this);

        Vector3 start = transform.position;
        Vector3 end = targetSlot.transform.position;
        float duration = Vector3.Distance(start, end) / MoveSpeed;

        transform.DOMove(end, duration).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            currentSlot = targetSlot;
            CheckTargetSeatMatch(targetSlot);
        });
    }

    protected virtual void AnimateCorridorMove(Seat targetSlot)
    {
        currentSlot.ClearCapybara();
        targetSlot.SetCapybara(this);

        Vector3 start = transform.position;
        Vector3 end = targetSlot.transform.position;
        float corridorOffset = 1.1f;

        SeatGroup fromGroup = currentSlot.groupOfSeat;
        SeatGroup toGroup = targetSlot.groupOfSeat;

        Vector3 corridorExit = GetCorridorExitPoint(fromGroup, currentSlot, corridorOffset, targetSlot);
        Vector3 step1 = corridorExit;
        Vector3 step2 = new Vector3(corridorExit.x, start.y, end.z);
        Vector3 step3 = end;

        float d1 = Vector3.Distance(start, step1);
        float d2 = Vector3.Distance(step1, step2);
        float d3 = Vector3.Distance(step2, step3);

        float dur1 = d1 / MoveSpeed;
        float dur2 = d2 / MoveSpeed;
        float dur3 = d3 / MoveSpeed;

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(step1, dur1).SetEase(Ease.Linear));
        seq.Append(transform.DOMove(step2, dur2).SetEase(Ease.Linear));
        seq.Append(transform.DOMove(step3, dur3).SetEase(Ease.Linear));
        seq.OnComplete(() =>
        {
            currentSlot = targetSlot;
            CheckTargetSeatMatch(targetSlot);
        });
    }

    protected virtual Vector3 GetCorridorExitPoint(SeatGroup seatGroup, Seat currentSeat, float offset, Seat targetSlot)
    {
        var corridorSeats = seatGroup.seatsInGroup.Where(s => s.isCorridorSide).ToList();

        if (!corridorSeats.Any())
            return currentSeat.transform.position;

        Seat appropriateCorridorSeat;
        bool shouldGoRight = GetMovementDirection(currentSeat, targetSlot);

        if (corridorSeats.Count == 1)
        {
            appropriateCorridorSeat = corridorSeats[0];
        }
        else
        {
            appropriateCorridorSeat = shouldGoRight
                ? corridorSeats.OrderByDescending(s => s.gridPosition.x).FirstOrDefault()
                : corridorSeats.OrderBy(s => s.gridPosition.x).FirstOrDefault();
        }

        Vector3 corridorPos = appropriateCorridorSeat.transform.position;
        Vector3 currentPos = currentSeat.transform.position;

        float xOffset;

        if (corridorSeats.Count == 1)
        {
            int corridorIndex = seatGroup.seatsInGroup.IndexOf(appropriateCorridorSeat);
            int groupMiddleIndex = seatGroup.seatsInGroup.Count / 2;

            // Eğer corridor seat grup dizisinin solundaysa offset sola, sağındaysa sağa
            xOffset = corridorIndex < groupMiddleIndex ? -offset : offset;
        }
        else
        {
            xOffset = shouldGoRight ? offset : -offset;
        }

        return new Vector3(corridorPos.x + xOffset, currentPos.y, currentPos.z);
    }


    protected virtual bool GetMovementDirection(Seat fromSeat, Seat toSeat)
    {
        return toSeat.transform.position.x > fromSeat.transform.position.x;
    }

    public void CheckTargetSeatMatch(Seat targetSlot)
    {
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
