using DG.Tweening;
using UnityEngine;

public class WagonVehicleAnimationController : VehicleAnimationController
{
    [Header("Zıplama Ayarları")]
    [Range(0f, 0.5f)] public float bounceHeight = 0.15f;
    public float bounceDuration = 1f;

    [Header("Salınım Ayarları")]
    [Range(0f, 5f)] public float swayAngle = 2f;
    public float swayDuration = 1.5f;

    private Tween bounceTween;
    private Tween swayTween;
    public override void AnimateVehicle()
    {
        // Yükselmeyi engellemek için sabit base değeri kullanılmaz, animasyon her seferinde orijinal scale kadar yukarı-aşağı gider.

        // Dikey zıplama (Y ekseninde yukarı-aşağı)
        bounceTween = transform.DOLocalMoveY(transform.localPosition.y + bounceHeight, bounceDuration / 2f)
            .SetEase(Ease.InOutQuad)
            .SetLoops(-1, LoopType.Yoyo);

        // Hafif öne-geri eğilme (X ekseninde rotasyon)
        swayTween = transform.DOLocalRotate(
            new Vector3(swayAngle, 0f, 0f), swayDuration / 2f, RotateMode.LocalAxisAdd)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void OnDisable()
    {
        bounceTween?.Kill();
        swayTween?.Kill();
    }
}
