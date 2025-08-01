using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainVehicleAnimationController : VehicleAnimationController
{
    [Header("Yalpalama (Z Rotasyon)")]
    [Range(0f, 5f)] public float tiltAngle = 2f;
    public float tiltDuration = 1.8f;

    [Header("Titreşim (Scale Varyasyonu)")]
    [Range(0f, 0.05f)] public float scaleIntensity = 0.015f;
    public float scaleDuration = 1.2f;

    private Tween tiltTween;
    private Tween scaleTween;

    public override void AnimateVehicle()
    {
        // Sağa-sola yalpalama
        tiltTween = transform.DOLocalRotate(
            new Vector3(0f, 0f, tiltAngle),
            tiltDuration / 2f,
            RotateMode.LocalAxisAdd)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        // Minik titreşim (hafifçe nefes alır gibi)
        scaleTween = transform.DOScale(
            transform.localScale + new Vector3(-scaleIntensity, scaleIntensity, -scaleIntensity),
            scaleDuration / 2f)
            .SetEase(Ease.InOutQuad)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void OnDisable()
    {
        tiltTween?.Kill();
        scaleTween?.Kill();
    }
}