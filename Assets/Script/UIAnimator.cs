using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public static class UIAnimator
{
    public static void FadeIn(CanvasGroup cg, float duration = 0.3f, float delay = 0f)
    {
        cg.alpha = 0f;
        cg.DOFade(1f, duration).SetDelay(delay);
    }

    public static void FadeOut(CanvasGroup cg, float duration = 0.3f, float delay = 0f)
    {
        cg.DOFade(0f, duration).SetDelay(delay);
    }

    public static void ScaleIn(Transform target, float duration = 0.4f, float delay = 0f, Ease ease = Ease.OutBack)
    {
        target.localScale = Vector3.zero;
        target.DOScale(1f, duration).SetEase(ease).SetDelay(delay);
    }

    public static void MoveFromX(Transform target, float fromX, float duration = 0.3f, Ease ease = Ease.OutExpo, float delay = 0f)
    {
        Vector3 original = target.localPosition;
        target.localPosition = new Vector3(fromX, original.y, original.z);
        target.DOLocalMoveX(original.x, duration).SetEase(ease).SetDelay(delay);
    }

    public static void MoveFromY(Transform target, float fromY, float duration = 0.3f, Ease ease = Ease.OutExpo, float delay = 0f)
    {
        Vector3 original = target.localPosition;
        target.localPosition = new Vector3(original.x, fromY, original.z);
        target.DOLocalMoveY(original.y, duration).SetEase(ease).SetDelay(delay);
    }

    public static void RotateLoop(Transform target, float duration = 6f)
    {
        target.DORotate(new Vector3(0, 0, 360f), duration, RotateMode.FastBeyond360)
              .SetEase(Ease.Linear)
              .SetLoops(-1);
    }

    public static void PulseFade(Image image, float minAlpha = 0.3f, float maxAlpha = 0.8f, float duration = 1f)
    {
        Color color = image.color;
        image.color = new Color(color.r, color.g, color.b, minAlpha);
        image.DOFade(maxAlpha, duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    public static void WobbleRotation(Transform target, float angleOffset = 5f, float duration = 1f)
    {
        Sequence seq = DOTween.Sequence();
        Vector3 originalEuler = target.localEulerAngles;

        seq.Append(target.DOLocalRotate(new Vector3(originalEuler.x, originalEuler.y, originalEuler.z + angleOffset), duration / 2f)
            .SetEase(Ease.InOutSine));
        seq.Append(target.DOLocalRotate(new Vector3(originalEuler.x, originalEuler.y, originalEuler.z - angleOffset), duration)
            .SetEase(Ease.InOutSine));
        seq.Append(target.DOLocalRotate(originalEuler, duration / 2f)
            .SetEase(Ease.InOutSine));

        seq.SetLoops(-1);
        seq.SetId("Wobble_" + target.GetInstanceID());
    }


}
