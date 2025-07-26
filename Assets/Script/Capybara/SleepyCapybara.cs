using UnityEngine;
using DG.Tweening;

public class SleepyCapybara : Capybara
{
    public override CapybaraType Type => CapybaraType.Sleepy;
    public override float SeatChangeTime => 2.5f; // Yavaş geçiş süresi

    public GameObject sleepEffect; // (isteğe bağlı) esneme/göz kapama gibi efekt objesi

    public override void SitSeat(Seat targetSlot)
    {
        if (!IsMovable() || targetSlot == null)
            return;

        currentSlot?.ClearCapybara();
        targetSlot.SetCapybara(this);

        currentSlot = targetSlot;

        // İsteğe bağlı görsel efekt
        if (sleepEffect != null)
        {
            sleepEffect.SetActive(true);
            // Otomatik olarak birkaç saniye sonra efekt kapanır
            Invoke(nameof(HideSleepEffect), 1.5f);
        }

        // Yavaş hareket
        transform.DOMove(targetSlot.transform.position, SeatChangeTime).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            CheckTargetSeatMatch(targetSlot);
        });
    }

    private void HideSleepEffect()
    {
        if (sleepEffect != null)
            sleepEffect.SetActive(false);
    }
}
