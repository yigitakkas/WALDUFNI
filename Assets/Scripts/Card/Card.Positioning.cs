using UnityEngine;
using DG.Tweening;

public partial class Card
{
    private bool IsCardOnPlayArea()
    {
        return _currentPlayArea != null && _currentPlayArea.IsPointInPlayArea(transform.position);
    }

    private void HandlePlayAreaPlacement()
    {
        Vector3 snapPosition = CalculateSnapPosition();
        MoveCardToPosition(snapPosition, dragMoveDuration, ResetShadowPosition);
    }

    private Vector3 CalculateSnapPosition()
    {
        if (_placedOnArea && (_placedArea == _currentPlayArea))
        {
            return _placedArea.GetCardPosition(this);
        }

        if (_placedArea != null && _placedArea != _currentPlayArea)
        {
            _placedArea.RemoveCard(this);
        }
        _currentPlayArea.PlaceCard(this);
        _placedArea = _currentPlayArea;
        _placedOnArea = true;

        return _currentPlayArea.GetSnapPosition(this);
    }

    private void MoveCardToPosition(Vector3 position, float duration, TweenCallback onComplete)
    {
        transform.DOMove(position, duration).SetEase(Ease.OutSine).OnComplete(onComplete);
    }

    private void ResetShadowPosition()
    {
        if (Shadow != null)
        {
            KillAndNullifyTween(ref _shadowMoveTween);
            _shadowMoveTween = Shadow.transform.DOLocalMove(_shadowOriginalLocalPosition, dragMoveDuration).SetEase(Ease.OutSine);
        }
    }

    private void ResetCardToOriginalPosition()
    {
        if (_placedOnArea)
        {
            _placedArea.RemoveCard(this);
            _placedArea = null;
        }
        _placedOnArea = false;

        transform.DOMove(_originalPosition, dragMoveDuration).SetEase(Ease.OutSine).OnComplete(() =>
        {
            if (_isHovered)
            {
                _cardVisual.transform.DOScale(_hoverScale, scaleDuration).SetEase(Ease.OutSine);
            }
            else
            {
                _cardVisual.transform.DOScale(_originalScale, scaleDuration).SetEase(Ease.OutSine);
                KillAndNullifyTween(ref _idleTween);
                StartIdleMovement();
            }
        });
    }

    private void AdjustChildSortingOrder(int orderOffset)
    {
        foreach (Transform child in _cardVisual.transform)
        {
            SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
            if (spriteRenderer)
            {
                spriteRenderer.sortingOrder += orderOffset;
            }
            else
            {
                Canvas canvas = child.GetComponent<Canvas>();
                if (canvas)
                {
                    canvas.sortingOrder += orderOffset;
                }
            }
        }
    }
}
