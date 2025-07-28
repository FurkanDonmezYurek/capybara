using System.Collections;
using UnityEngine;

public class ChildCapybara : Capybara
{
    public override CapybaraType Type => CapybaraType.Child;
    public override float MoveSpeed => 3f;
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
        Debug.Log("Started auto move coroutine for: " + name);
        while (!isLocked)
        {
            yield return new WaitForSeconds(Random.Range(minMoveInterval, maxMoveInterval));

            if (!IsMovable())
            {
                Debug.Log(name + " is not movable!");
                continue;
            }

            Seat target = GameManager.Instance.GetRandomAvailableSeat();

            if (target == null || target == currentSlot)
            {
                Debug.Log("Auto move skipped!. Target slot is: " + target.name);
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
