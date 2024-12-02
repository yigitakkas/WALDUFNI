using UnityEngine;
using DG.Tweening;
using System.Collections;

public class Card : MonoBehaviour
{
    private Tween _hoverTween;
    private Tween _scaleTween;
    private Tween _dragTween;
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

    private void Start()
    {
        InitializeCard();
        StartIdleMovement();
    }

    private void InitializeCard()
    {
        _originalScale = transform.localScale;
        _shadowOriginalLocalPosition = Shadow != null ? Shadow.transform.localPosition : Vector3.zero;
        _hoverScale = _originalScale * 1.05f;
        _movingScale = _originalScale * 1.05f * 1.10f;
        _mainCamera = Camera.main;
    }

    private void OnMouseDown()
    {
        _offset = transform.position - GetMouseWorldPosition();
        _isDragging = true;
        KillTweens();
        transform.DOScale(_movingScale, scaleDuration).SetEase(Ease.OutSine);
    }

    private void OnMouseDrag()
    {
        Vector3 targetPosition = GetMouseWorldPosition() + _offset;
        targetPosition.z = transform.position.z;
        if(_dragTween == null || !_dragTween.IsPlaying())
        {
            _dragTween = transform.DOMove(targetPosition, dragMoveDuration).SetEase(Ease.Linear);
        }

        if (Shadow != null && (_shadowMoveTween == null || !_shadowMoveTween.IsPlaying()))
        {
            Vector3 shadowTargetPosition = targetPosition;
            shadowTargetPosition.y += shadowOffsetY;
            shadowTargetPosition.x = targetPosition.x + _shadowOriginalLocalPosition.x ;
            shadowTargetPosition.z = 1;
            _shadowMoveTween = Shadow.transform.DOMove(shadowTargetPosition, dragMoveDuration).SetEase(Ease.Linear);
        }
    }

    private void OnMouseUp()
    {
        if (Shadow != null)
        {
            _shadowMoveTween?.Kill();
            _shadowMoveTween = Shadow.transform.DOLocalMove(_shadowOriginalLocalPosition, dragMoveDuration).SetEase(Ease.OutSine);
        }
        _isDragging = false;
        StartIdleMovement();
        if(_isHovered)
        {
            transform.DOScale(_hoverScale, scaleDuration).SetEase(Ease.OutSine)
                .OnComplete(() => CheckDragOrMousePos());
        }
    }

    private void CheckDragOrMousePos()
    {
        if(!_isDragging)
        {
            CheckMousePos();
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
        _isHovered = false;
        if (!_isDragging)
        {
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
        if(_hoverTween == null || !_hoverTween.IsPlaying() && (_scaleTween == null || !_scaleTween.IsPlaying()))
        {
            return true;
        } else
        {
            return false;
        }

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
        _dragTween?.Kill();
        _idleTween?.Kill();
        _shadowMoveTween?.Kill();
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Mathf.Abs(_mainCamera.transform.position.z - transform.position.z);
        return _mainCamera.ScreenToWorldPoint(mousePosition);
    }
}
