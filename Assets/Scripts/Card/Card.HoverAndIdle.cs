using UnityEngine;
using DG.Tweening;

public partial class Card
{
    public void StartHoverEffect()
    {
        if (CheckHoverConditions())
        {
            KillTweens();
            StopIdleMovement();
            _hoverTween = _cardVisual.transform.DOLocalRotate(new Vector3(0, 0, Random.Range(hoverRotationAngleMin, hoverRotationAngleMax)), hoverDuration)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .OnComplete(() => StartScaleEffect());
        }
    }

    private bool CheckHoverConditions()
    {
        return _hoverTween == null || (!_hoverTween.IsPlaying() && (_scaleTween == null || !_scaleTween.IsPlaying()));
    }

    private void StartScaleEffect()
    {
        KillTweens();
        _cardVisual.transform.rotation = Quaternion.identity;
        _scaleTween = _cardVisual.transform.DOScale(_hoverScale, scaleDuration)
            .SetEase(Ease.OutSine)
            .OnStart(() => _hoverTween = null)
            .OnComplete(() => CheckMousePos());
    }

    private void CheckMousePos()
    {
        if ((!_isHovered && !_isDragging) || Played)
        {
            StopHoverEffect();
        }
    }

    public void StopHoverEffect()
    {
        KillTweens();
        _cardVisual.transform.rotation = Quaternion.identity;
        _scaleTween = _cardVisual.transform.DOScale(_originalScale, scaleDuration).SetEase(Ease.OutSine);
        StartIdleMovement();
    }

    private void StartIdleMovement()
    {
        _idleTween = _cardVisual.transform.DOLocalRotate(new Vector3(idleTiltAngle, idleTiltAngle, Random.Range(idleRotationAngleMin, idleRotationAngleMax)), idleMovementDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void StopIdleMovement()
    {
        KillAndNullifyTween(ref _idleTween);
    }

    private void KillTweens()
    {
        KillAndNullifyTween(ref _hoverTween);
        KillAndNullifyTween(ref _scaleTween);
        KillAndNullifyTween(ref _idleTween);
        KillAndNullifyTween(ref _shadowMoveTween);
        _playedUpdated = false;
    }

    private void KillAndNullifyTween(ref Tween tween)
    {
        if (tween != null)
        {
            tween.Kill();
            tween = null;
        }
    }
}
