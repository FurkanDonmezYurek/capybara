using DG.Tweening;
using UnityEngine;

public class ShipVehicleAnimationController : VehicleAnimationController
{
    [Header("Yana Sallanma (Z Rotasyon)")]
    [Range(0f, 5f)] public float sideTiltAngle = 2.5f;
    public float sideTiltDuration = 2.2f;

    [Header("Öne/Arkaya Sallanma (X Rotasyon)")]
    [Range(0f, 3f)] public float frontBackAngle = 1.5f;
    public float frontBackDuration = 2.6f;

    [Header("Yukarı-Aşağı Dalga (Opsiyonel)")]
    [Range(0f, 0.3f)] public float floatHeight = 0.1f;
    public float floatDuration = 2.4f;

    private Tween zTiltTween;
    private Tween xTiltTween;
    private Tween floatTween;

    public override void AnimateVehicle()
    {
        // Yana-yana salınım (Z rotasyon)
        zTiltTween = transform.DOLocalRotate(
            new Vector3(0f, 0f, sideTiltAngle),
            sideTiltDuration / 2f,
            RotateMode.LocalAxisAdd)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        // Öne-arkaya salınım (X rotasyon)
        xTiltTween = transform.DOLocalRotate(
            new Vector3(frontBackAngle, 0f, 0f),
            frontBackDuration / 2f,
            RotateMode.LocalAxisAdd)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        // Yukarı-aşağı (Y ekseninde dalgalanma)
        floatTween = transform.DOLocalMoveY(
            transform.localPosition.y + floatHeight,
            floatDuration / 2f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void OnDisable()
    {
        zTiltTween?.Kill();
        xTiltTween?.Kill();
        floatTween?.Kill();
    }
}
