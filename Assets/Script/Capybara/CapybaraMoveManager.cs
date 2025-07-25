using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CapybaraMoveManager : MonoBehaviour
{
    public static CapybaraMoveManager Instance;

    private Capybara selectedCapybara;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SelectCapybara(Capybara capy)
    {
        if (capy == null || !capy.IsMovable()) return;
        selectedCapybara = capy;
    }

    public void TryMoveSelectedTo(SeatSlot targetSlot)
    {
        if (selectedCapybara == null || targetSlot == null) return;

        selectedCapybara.MoveTo(targetSlot);
        selectedCapybara = null;
    }

    public SeatSlot GetFirstAvailableSlot()
    {
        var allSlots = GridManager.Instance.GetAllSlots();
        return allSlots.FirstOrDefault(slot => slot.IsEmpty());
    }

    public SeatSlot FindRandomFreeSlot()
    {
        var free = GridManager.Instance.GetAllSlots().Where(s => s.IsEmpty()).ToList();
        if (free.Count == 0) return null;
        return free[Random.Range(0, free.Count)];
    }

    public void ClearSelection()
    {
        selectedCapybara = null;
    }

    public Capybara GetSelected() => selectedCapybara;
}
