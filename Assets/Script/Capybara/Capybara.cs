using System.Collections.Generic;
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

        transform
            .DOMove(end, duration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                currentSlot = targetSlot;
                CheckTargetSeatMatch(targetSlot);
            });
    }

    protected virtual void AnimateCorridorMove(Seat targetSlot)
    {
        currentSlot.ClearCapybara();
        targetSlot.SetCapybara(this);

        GridSystem gridSystem = FindObjectOfType<GridSystem>();
        if (gridSystem == null)
        {
            Debug.LogError("GridSystem not found!");
            return;
        }

        Vector3 start = transform.position;
        Vector3 end = targetSlot.transform.position;

        SeatGroup fromGroup = currentSlot.groupOfSeat;
        SeatGroup toGroup = targetSlot.groupOfSeat;

        int fromX = fromGroup.groupX;
        int toX = toGroup.groupX;
        int y = fromGroup.groupY; // corridor satırı

        Vector3 fromExit = GetCorridorExitPoint(currentSlot, targetSlot);
        Vector3 toEntry = GetCorridorExitPoint(targetSlot, currentSlot);

        List<Vector3> pathPoints = new();
        pathPoints.Add(start); // A - Başlangıç noktası
        pathPoints.Add(fromExit); // B - Çıkış corridor noktası

        // corridor üzerindeki geçişler (gruplar arasında)
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

        pathPoints.Add(toEntry); // C - Hedef grubun corridor giriş noktası
        pathPoints.Add(end); // D - Hedef koltuk

        // Animasyon
        Sequence seq = DOTween.Sequence();
        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            float dist = Vector3.Distance(pathPoints[i], pathPoints[i + 1]);
            float dur = dist / MoveSpeed;
            seq.Append(transform.DOMove(pathPoints[i + 1], dur).SetEase(Ease.Linear));
        }

        seq.OnComplete(() =>
        {
            currentSlot = targetSlot;
            CheckTargetSeatMatch(targetSlot);
            if (Application.isPlaying)
                GameManager.Instance.CheckGameCondition();
        });
    }

    protected virtual Vector3 GetCorridorExitPoint(Seat seat, Seat targetSeat)
    {
        GridSystem gridSystem = FindObjectOfType<GridSystem>();
        if (gridSystem == null || gridSystem.pathPointsGrid == null)
            return seat.transform.position;

        int groupX = seat.groupOfSeat.groupX;
        int groupY = seat.groupOfSeat.groupY;

        List<Vector3> candidatePoints = new();

        // GridSystem'deki corridor noktaları arasında kendi grubunun sağındaki corridor da olabilir
        if (groupX > 0)
        {
            string leftKey = (groupX - 1).ToString();
            if (
                gridSystem.pathPointsGrid.TryGetValue(leftKey, out var yDict)
                && yDict.TryGetValue(groupY.ToString(), out var point)
            )
            {
                candidatePoints.Add(point);
            }
        }

        string currentKey = groupX.ToString();
        if (
            gridSystem.pathPointsGrid.TryGetValue(currentKey, out var currentYDict)
            && currentYDict.TryGetValue(groupY.ToString(), out var currentPoint)
        )
        {
            candidatePoints.Add(currentPoint);
        }

        // En yakın corridor noktasını hedefe göre seç
        Vector3 targetPos = targetSeat.transform.position;
        Vector3 bestPoint = candidatePoints
            .OrderBy(p => Vector3.Distance(p, targetPos))
            .FirstOrDefault();

        return new Vector3(bestPoint.x, seat.transform.position.y, seat.transform.position.z);
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
