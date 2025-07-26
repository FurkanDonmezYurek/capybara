using System.Collections;
using UnityEngine;

public class ChildCapybara : Capybara
{
    public override CapybaraType Type => CapybaraType.Child;
    public override float SeatChangeTime => 0.5f;

    public float moveInterval = 10f;

    private Coroutine autoMoveRoutine;

    private void Start()
    {
        // Start changing seats automatically on start for now. 
        StartAutoMoving();
    }

    public void StartAutoMoving()
    {
        autoMoveRoutine = StartCoroutine(AutoMoveCoroutine());
    }

    public void StopAutoMoving()
    {
        if (autoMoveRoutine != null)
        {
            StopCoroutine(autoMoveRoutine);
        }
    }

    IEnumerator AutoMoveCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(moveInterval);

            if (!IsMovable())
                continue;

            Seat target = GameManager.Instance.GetRandomAvailableSeat();

            if (target == null || target == currentSlot)
                continue;

            SitSeat(target);
        }
    }

    private void OnDestroy()
    {
        if (autoMoveRoutine != null)
            StopCoroutine(autoMoveRoutine);
    }
}
