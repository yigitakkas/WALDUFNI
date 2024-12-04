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
    private bool _placedOnArea=false;

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

    private void OnMouseDown()
    {
        foreach (Transform child in transform)
        {
            SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
            if (spriteRenderer)
            {
                spriteRenderer.sortingOrder+=2;
            }
            else
            {
                Canvas canvas = child.GetComponent<Canvas>();
                if (canvas)
                {
                    canvas.sortingOrder+=2;
                }
            }
        }
        _offset = transform.position - GetMouseWorldPosition();
        _isDragging = true;
        KillTweens();
        transform.DOScale(_movingScale, scaleDuration).SetEase(Ease.OutSine);
    }

    private void OnMouseDrag()
    {
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
        foreach (Transform child in transform)
        {
            SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
            if(spriteRenderer)
            {
                spriteRenderer.sortingOrder-=2;
            } else
            {
                Canvas canvas = child.GetComponent<Canvas>();
                if(canvas)
                {
                    canvas.sortingOrder-=2;
                }
            }
        }
        _isDragging = false;
        _hoverTween?.Kill();

        if (_currentPlayArea != null && _currentPlayArea.IsPointInPlayArea(transform.position))
        {
            Vector3 snapPosition = Vector3.zero;
            if(_placedOnArea && (_placedArea == _currentPlayArea))
            {
                snapPosition = _placedArea.GetCardPosition(this);

            } else
            {
                if(_placedArea != null && _placedArea != _currentPlayArea)
                {
                    _placedArea.RemoveCard(this);
                }
                snapPosition = _currentPlayArea.GetSnapPosition(this);
                _currentPlayArea.PlaceCard(this);
                _placedArea = _currentPlayArea;
                _placedOnArea = true;
            }
            transform.DOMove(snapPosition, dragMoveDuration).SetEase(Ease.OutSine).OnComplete(() =>
            {
                if (Shadow != null)
                {
                    _shadowMoveTween?.Kill();
                    _shadowMoveTween = Shadow.transform.DOLocalMove(_shadowOriginalLocalPosition, dragMoveDuration).SetEase(Ease.OutSine);
                }
            });
        }
        else
        {
            if(_placedOnArea)
            {
                _placedArea.RemoveCard(this);
                _placedArea = null;
            }
            _placedOnArea = false;
            if (Shadow != null)
            {
                _shadowMoveTween?.Kill();
                _shadowMoveTween = Shadow.transform.DOLocalMove(_shadowOriginalLocalPosition, dragMoveDuration).SetEase(Ease.OutSine);
            }

            transform.DOMove(_originalPosition, dragMoveDuration).SetEase(Ease.OutSine).OnComplete(() =>
            {
                if (_isHovered)
                {
                    transform.DOScale(_hoverScale, scaleDuration).SetEase(Ease.OutSine);
                }
                else
                {
                    transform.DOScale(_originalScale, scaleDuration).SetEase(Ease.OutSine);
                    _idleTween?.Kill();
                    StartIdleMovement();
                }
            });
        }
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
        _idleTween?.Kill();
        _idleTween = null;
    }

    private void KillTweens()
    {
        _hoverTween?.Kill();
        _scaleTween?.Kill();
        _idleTween?.Kill();
        _shadowMoveTween?.Kill();
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