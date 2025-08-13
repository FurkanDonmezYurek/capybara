using System.Collections;
using UnityEngine;

public class ChildCapybara : Capybara
{
    public override CapybaraType Type => CapybaraType.Child;
    public override float MoveSpeed => 6.5f;
    public float minMoveInterval = 10f;
    public float maxMoveInterval = 20f;

    private Coroutine autoMoveRoutine;

    public override void Start()
    {
        base.Start();
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
        while (!isLocked)
        {
            yield return new WaitForSeconds(Random.Range(minMoveInterval, maxMoveInterval));

            if (!IsMovable())
            {
                continue;
            }

            yield return new WaitUntil(() => GameManager.Instance.movingCapybaras.Count == 0);

            Seat target = GameManager.Instance.GetRandomAvailableSeat();

            if (target == null || target == currentSlot)
            {
                continue;
            }
            if (GameManager.Instance.IsCorrectMove(target, currentSlot))
                SitSeat(target);
        }
    }

    private void OnDestroy()
    {
        if (autoMoveRoutine != null)
            StopCoroutine(autoMoveRoutine);
    }
}
