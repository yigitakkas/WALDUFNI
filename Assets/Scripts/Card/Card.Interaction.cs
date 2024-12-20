using UnityEngine;
using DG.Tweening;

public partial class Card
{
    private void OnMouseDown()
    {
        Debug.Log("OnMouseDown" + name);
        if (Played || !EnergyManager.Instance.CheckIfMovable(GetComponent<CardDisplay>().Energy, this) || _roundPlaying) return;
        AdjustChildSortingOrder(2);
        _offset = transform.position - GetMouseWorldPosition();
        _isDragging = true;
        KillTweens();
        _cardVisual.transform.DOScale(_movingScale, scaleDuration).SetEase(Ease.OutSine);
    }

    private void OnMouseUp()
    {
        Debug.Log("OnMouseUp" + name);
        if (Played || !EnergyManager.Instance.CheckIfMovable(GetComponent<CardDisplay>().Energy, this) || _roundPlaying) return;
        AdjustChildSortingOrder(-2);
        _isDragging = false;
        KillAndNullifyTween(ref _hoverTween);

        if (IsCardOnPlayArea() && _currentPlayArea.CheckSnapPointsAvailability())
        {
            if (!_placedOnArea)
            {
                _placedOnArea = true;
                EnergyManager.Instance.DecreaseEnergy(GetComponent<CardDisplay>().Energy, player: true);
            }
            HandlePlayAreaPlacement();
        }
        else
        {
            if (_placedOnArea)
            {
                _placedOnArea = false;
                _placedArea.RemoveCard(this);
                _placedArea = null;
                EnergyManager.Instance.IncreaseEnergy(GetComponent<CardDisplay>().Energy, player: true);
            }
            ResetCardToOriginalPosition();
        }
    }

    private void OnMouseEnter()
    {
        Debug.Log("OnMouseEnter" + name);
        if (!_isDragging && !_isHovered)
        {
            _isHovered = true;
            ShowCardTooltip();
            if (EnergyManager.Instance.CheckIfMovable(GetComponent<CardDisplay>().Energy, this) && !Played && !_roundPlaying)
                StartHoverEffect();
        }
    }

    private void OnMouseExit()
    {
        Debug.Log("OnMouseExit" + name);
        if (!_isDragging && _isHovered)
        {
            _isHovered = false;
            HideCardTooltip();
            if (EnergyManager.Instance.CheckIfMovable(GetComponent<CardDisplay>().Energy, this) && !Played && !_roundPlaying)
                StopHoverEffect();
        }
    }

    private void OnMouseDrag()
    {
        Debug.Log("OnMouseDrag" + name);
        if (!_isDragging || Played || !EnergyManager.Instance.CheckIfMovable(GetComponent<CardDisplay>().Energy, this) || _roundPlaying) return;

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
