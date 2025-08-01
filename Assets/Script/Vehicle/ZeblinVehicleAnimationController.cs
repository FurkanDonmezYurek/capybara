using DG.Tweening;
using UnityEngine;

public class ZeblinVehicleAnimationController : VehicleAnimationController
{
    [Header("Yukarı-Aşağı Salınım")]
    [Range(0f, 0.5f)] public float floatHeight = 0.2f;
    public float floatDuration = 2f;

    [Header("Hafif Eğilme (Z Rotasyon)")]
    [Range(0f, 5f)] public float tiltAngle = 3f;
    public float tiltDuration = 2.5f;

    private Tween floatTween;
    private Tween tiltTween;

    public override void AnimateVehicle()
    {
        // Yukarı-aşağı süzülme (float)
        floatTween = transform.DOLocalMoveY(transform.localPosition.y + floatHeight, floatDuration / 2f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        // Hafif sağ-sol eğilme (tilt)
        tiltTween = transform.DOLocalRotate(
            new Vector3(0f, 0f, tiltAngle),
            tiltDuration / 2f,
            RotateMode.LocalAxisAdd)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void OnDisable()
    {
        floatTween?.Kill();
        tiltTween?.Kill();
    }
}
