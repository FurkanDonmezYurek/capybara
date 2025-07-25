
using UnityEngine;

public class ChildCapybara : Capybara
{
    public override CapybaraType Type => CapybaraType.Child;
    public override float SeatChangeTime => 0.5f;
    private float timer = 0f;
    public float moveDelay = 5f;

    protected override void Update()
    {
        base.Update();
        if (currentSlot == null) return;

        timer += Time.deltaTime;
        if (timer >= moveDelay)
        {
            timer = 0;
            var targetSlot = CapybaraMoveManager.Instance.FindRandomFreeSlot();
            if (targetSlot != null)
                MoveTo(targetSlot);
        }
    }
}
