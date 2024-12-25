using UnityEngine;
using DG.Tweening;

public partial class Card
{
    private void OnMouseDown()
    {
        if (!EnergyManager.Instance.CheckIfMovable(Energy, this) || Played) SoundManager.Instance.PlaySFX(SoundManager.Instance.CardUnselectableSound);
        if (Played || !EnergyManager.Instance.CheckIfMovable(Energy, this) || _roundPlaying || Time.timeScale==0) return;
        AdjustChildSortingOrder(3);
        _offset = transform.position - GetMouseWorldPosition();
        _isDragging = true;
        SoundManager.Instance.PlaySFX(SoundManager.Instance.CardPickupSound);
        KillTweens();
        _cardVisual.transform.DOScale(_movingScale, scaleDuration).SetEase(Ease.OutSine);
    }

    private void OnMouseUp()
    {
        if (Played || !EnergyManager.Instance.CheckIfMovable(Energy, this) || _roundPlaying || Time.timeScale == 0) return;
        AdjustChildSortingOrder(-3);
        _isDragging = false;
        KillAndNullifyTween(ref _hoverTween);

        if (IsCardOnPlayArea() && _currentPlayArea.CheckSnapPointsAvailability())
        {
            if (!_placedOnArea)
            {
                _placedOnArea = true;
                EnergyManager.Instance.DecreaseEnergy(Energy, player: true);
            }
            SoundManager.Instance.PlaySFX(SoundManager.Instance.CardPlayedSound);
            HandlePlayAreaPlacement();
        }
        else
        {
            if (_placedOnArea)
            {
                _placedOnArea = false;
                _placedArea.RemoveCard(this);
                _placedArea = null;
                EnergyManager.Instance.IncreaseEnergy(Energy, player: true);
            }
            SoundManager.Instance.PlaySFX(SoundManager.Instance.CardErrorSound);
            ResetCardToOriginalPosition();
        }
    }

    private void OnMouseEnter()
    {
        if (Time.timeScale == 0) return;
        if (!_isDragging && !_isHovered)
        {
            _isHovered = true;
            ShowCardTooltip();
            if (EnergyManager.Instance.CheckIfMovable(Energy, this) && !Played && !_roundPlaying)
                StartHoverEffect();
        }
    }

    private void OnMouseExit()
    {
        if (Time.timeScale == 0) return;
        if (!_isDragging && _isHovered)
        {
            _isHovered = false;
            HideCardTooltip();
            if (EnergyManager.Instance.CheckIfMovable(Energy, this) && !Played && !_roundPlaying)
                StopHoverEffect();
        }
    }

    private void OnMouseDrag()
    {
        if (!_isDragging || Played || !EnergyManager.Instance.CheckIfMovable(Energy, this) || _roundPlaying || Time.timeScale == 0) return;

        Vector3 targetPosition = GetMouseWorldPosition() + _offset;
        targetPosition.z = transform.position.z;
        transform.position = targetPosition;

        if (Shadow != null)
        {
            Vector3 shadowTargetPosition = targetPosition;
            shadowTargetPosition.x += (_cardVisual.transform.position.x > 0 ? 1 : -1) * shadowOffsetXFactor;
            shadowTargetPosition.y += shadowOffsetY;
            shadowTargetPosition.z = _cardVisual.transform.position.z + 0.1f;
            Shadow.transform.position = shadowTargetPosition;
        }
    }
}
