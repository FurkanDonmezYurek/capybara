using System.Collections;
using DG.Tweening;
using UnityEngine;

public class SleepyCapybara : Capybara
{
    public override CapybaraType Type => CapybaraType.Sleepy;
    public override float MoveSpeed => 3f;
    public bool isAsleep = true;
    public float sleepDuration = 5f; // Uyku süresi
    public float wakeUpDuration = 6f; // Uyanma süresi
    public GameObject sleepEffect;

    //<summary>
    //This class represents a sleepy capybara that alternates between sleeping and waking up.
    //</summary>

    public override void Start()
    {
        base.Start();
        StartCoroutine(StartSleepCycle());
    }

    IEnumerator StartSleepCycle()
    {
        while (true)
        {
            if (isAsleep)
            {
                yield return new WaitForSeconds(sleepDuration); // Uyku süresi
                WakeUp();
            }
            else
            {
                yield return new WaitForSeconds(wakeUpDuration); // Uyanıklık süresi
                Sleep();
            }
        }
    }

    void OnDestroy()
    {
        HideSleepEffect();
        StopAllCoroutines();
    }
    public void WakeUp()
    {
        isAsleep = false;
        HideSleepEffect();
    }

    public void Sleep()
    {
        isAsleep = true;
        if (sleepEffect != null)
        {
            sleepEffect.SetActive(true);
        }
    }

    public override bool IsMovable()
    {
        return !isLocked && !isFrozen && !isAsleep;
    }

    private void HideSleepEffect()
    {
        if (sleepEffect != null)
            sleepEffect.SetActive(false);
    }
}
