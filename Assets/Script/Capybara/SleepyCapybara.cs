using DG.Tweening;
using UnityEngine;

public class SleepyCapybara : Capybara
{
    public override CapybaraType Type => CapybaraType.Sleepy;
    public override float MoveSpeed => 1f;
    public GameObject sleepEffect; // (isteğe bağlı) esneme/göz kapama gibi efekt objesi

    public override void Start()
    {
        base.Start();
        if (sleepEffect != null)
        {
            sleepEffect.SetActive(true);
        }
    }

    private void HideSleepEffect()
    {
        if (sleepEffect != null)
            sleepEffect.SetActive(false);
    }
}
