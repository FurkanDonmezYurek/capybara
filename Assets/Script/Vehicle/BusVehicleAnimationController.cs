using UnityEngine;
using DG.Tweening;

public class BusVehicleAnimationController : VehicleAnimationController
{
    [Header("Şişme (Scale)")]
    [Range(0f, 0.2f)] public float scaleIntensity = 0.035f;
    public float scaleCycleDuration = 1.4f;

    [Header("Sallanma (Tilt)")]
    [Range(0f, 10f)] public float tiltAngle = 3f;
    public float tiltDuration = 1.2f;

    private Tween scaleTween;
    private Tween tiltTween;

    public override void AnimateVehicle()
    {
        // Şişme (nefes alma efekti)
        scaleTween = DOTween.Sequence()
            .Append(transform.DOScale(transform.localScale + new Vector3(scaleIntensity, -scaleIntensity, scaleIntensity), scaleCycleDuration / 2f).SetEase(Ease.InOutQuad))
            .Append(transform.DOScale(transform.localScale, scaleCycleDuration / 2f).SetEase(Ease.InOutQuad))
            .SetLoops(-1);

        // Sadece Z ekseninde sağa-sola eğilme
        float currentZ = transform.localEulerAngles.z;

        tiltTween = DOTween.Sequence()
            .Append(transform.DOLocalRotate(new Vector3(0, 180, currentZ + tiltAngle), tiltDuration / 2f).SetEase(Ease.InOutSine))
            .Append(transform.DOLocalRotate(new Vector3(0, 180, currentZ - tiltAngle), tiltDuration).SetEase(Ease.InOutSine))
            .Append(transform.DOLocalRotate(new Vector3(0, 180, currentZ), tiltDuration / 2f).SetEase(Ease.InOutSine))
            .SetLoops(-1);
    }
    private void OnDisable()
    {
        scaleTween?.Kill();
        tiltTween?.Kill();

    }
}
