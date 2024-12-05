using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public ICardAbility CardAbility { get; private set; }

    private Tween _hoverTween;
    private Tween _scaleTween;
    private Tween _idleTween;
    private Tween _shadowMoveTween;
    private Vector3 _originalScale;
    private Vector3 _shadowOriginalLocalPosition;
    private Vector3 _hoverScale;
    private Vector3 _offset;
    private Vector3 _movingScale;
    private Camera _mainCamera;
    private bool _isDragging = false;
    private bool _isHovered = false;
    public GameObject Shadow;
    private Collider2D _collider;

    [Header("Hover Effect Settings")]
    public float hoverRotationAngleMin = -5f;
    public float hoverRotationAngleMax = 5f;
    public float hoverDuration = 0.2f;
    public float scaleDuration = 0.2f;

    [Header("Drag Effect Settings")]
    public float dragMoveDuration = 0.05f;
    public float shadowOffsetY = -0.05f;
    public float shadowOffsetXFactor = 0.1f;

    [Header("Idle Movement Settings")]
    public float idleRotationAngleMin = -1f;
    public float idleRotationAngleMax = 1f;
    public float idleTiltAngle = 1f;
    public float idleMovementDuration = 2f;

    public bool IsDragging => _isDragging;
    private Vector3 _originalPosition;
    private PlayArea _currentPlayArea;
    private PlayArea _placedArea;
    private bool _placedOnArea = false;

    private void Start()
    {
        InitializeCard();
        StartIdleMovement();
        _originalPosition = transform.position;
    }
    public void SetAbility(ICardAbility ability)
    {
        CardAbility = ability;
    }

    public void ActivateAbility()
    {
        CardAbility?.ApplyEffect();
    }

    private void InitializeCard()
    {
        _originalScale = transform.localScale;
        _shadowOriginalLocalPosition = Shadow != null ? Shadow.transform.localPosition : Vector3.zero;
        _hoverScale = _originalScale * 1.05f;
        _movingScale = _originalScale * 1.05f * 1.10f;
        _mainCamera = Camera.main;
        _collider = GetComponent<BoxCollider2D>();
    }

    public void DestroyCollider()
    {
        BoxCollider2D newCollider = GetComponent<BoxCollider2D>();
        newCollider.enabled = false;
    }

    private void OnMouseDown()
    {
        AdjustChildSortingOrder(2);
        _offset = transform.position - GetMouseWorldPosition();
        _isDragging = true;
        KillTweens();
        transform.DOScale(_movingScale, scaleDuration).SetEase(Ease.OutSine);
    }

    private void OnMouseDrag()
    {
        if (!_isDragging) return;

        Vector3 targetPosition = GetMouseWorldPosition() + _offset;
        targetPosition.z = transform.position.z;
        transform.position = targetPosition;

        if (Shadow != null)
        {
            Vector3 shadowTargetPosition = targetPosition;
            shadowTargetPosition.x += (transform.position.x > 0 ? 1 : -1) * shadowOffsetXFactor;
            shadowTargetPosition.y += shadowOffsetY;
            shadowTargetPosition.z = transform.position.z + 0.1f;
            Shadow.transform.position = shadowTargetPosition;
        }
    }

    private void OnMouseUp()
    {
        AdjustChildSortingOrder(-2);
        _isDragging = false;
        KillAndNullifyTween(ref _hoverTween);

        if (IsCardOnPlayArea())
        {
            HandlePlayAreaPlacement();
        }
        else
        {
            ResetCardToOriginalPosition();
        }
    }
    private void AdjustChildSortingOrder(int orderOffset)
    {
        foreach (Transform child in transform)
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
                transform.DOScale(_hoverScale, scaleDuration).SetEase(Ease.OutSine);
            }
            else
            {
                transform.DOScale(_originalScale, scaleDuration).SetEase(Ease.OutSine);
                KillAndNullifyTween(ref _idleTween);
                StartIdleMovement();
            }
        });
    }

    private void OnMouseOver()
    {
        if (!_isDragging && !_isHovered)
        {
            _isHovered = true;
            StartHoverEffect();
        }
    }

    private void OnMouseExit()
    {
        if (!_isDragging)
        {
            _isHovered = false;
            StopHoverEffect();
        }
    }

    public void StartHoverEffect()
    {
        if (CheckHoverConditions())
        {
            StopIdleMovement();
            _hoverTween = transform.DOLocalRotate(new Vector3(0, 0, Random.Range(hoverRotationAngleMin, hoverRotationAngleMax)), hoverDuration)
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
        transform.rotation = Quaternion.identity;
        _scaleTween = transform.DOScale(_hoverScale, scaleDuration)
            .SetEase(Ease.OutSine)
            .OnStart(() => _hoverTween = null)
            .OnComplete(() => CheckMousePos());
    }

    private void CheckMousePos()
    {
        if (!_isHovered && !_isDragging)
        {
            StopHoverEffect();
        }
    }

    public void StopHoverEffect()
    {
        KillTweens();
        transform.rotation = Quaternion.identity;
        _scaleTween = transform.DOScale(_originalScale, scaleDuration).SetEase(Ease.OutSine);
        StartIdleMovement();
    }

    private void StartIdleMovement()
    {
        _idleTween = transform.DOLocalRotate(new Vector3(idleTiltAngle, idleTiltAngle, Random.Range(idleRotationAngleMin, idleRotationAngleMax)), idleMovementDuration)
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
    }

    private void KillAndNullifyTween(ref Tween tween)
    {
        tween.Kill();
        tween = null;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Mathf.Abs(_mainCamera.transform.position.z - transform.position.z);
        return _mainCamera.ScreenToWorldPoint(mousePosition);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out PlayArea playArea))
        {
            _currentPlayArea = playArea;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out PlayArea playArea) && _currentPlayArea == playArea)
        {
            _currentPlayArea = null;
        }
    }
}
