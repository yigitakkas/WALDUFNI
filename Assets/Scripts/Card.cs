using UnityEngine;
using DG.Tweening;

public class Card : MonoBehaviour
{
    private Tween _hoverTween;
    private Tween _scaleTween;
    private Tween _dragTween;
    private Vector3 _originalScale;
    private Vector3 _offset;
    private Camera _mainCamera;
    private bool _isDragging = false;
    private bool _isHovered = false;
    private bool _justPutDown = false;

    [Header("Hover Effect Settings")]
    public float hoverRotationAngle = 5f;
    public float hoverDuration = 0.2f;
    public float hoverScaleMultiplier = 1.1f;
    public float scaleDuration = 0.3f;

    [Header("Drag Effect Settings")]
    public float dragMoveDuration = 0.05f;

    private void Start()
    {
        _originalScale = transform.localScale;
        _mainCamera = Camera.main;
    }

    private void OnMouseDown()
    {
        _offset = transform.position - GetMouseWorldPosition();
        _isDragging = true;
        _dragTween?.Kill();
    }

    private void OnMouseDrag()
    {
        Vector3 targetPosition = GetMouseWorldPosition() + _offset;
        targetPosition.z = transform.position.z;
        _dragTween = transform.DOMove(targetPosition, dragMoveDuration).SetEase(Ease.Linear);
    }

    private void OnMouseUp()
    {
        _isDragging = false;
        _justPutDown = true;
    }

    private void OnMouseOver()
    {
        if (!_isDragging && !_isHovered && !_justPutDown)
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
            _justPutDown = false;
            StopHoverEffect();
        }
    }

    public void StartHoverEffect()
    {
        if (_hoverTween == null || !_hoverTween.IsPlaying() && (_scaleTween == null || !_scaleTween.IsPlaying()))
        {
            _hoverTween = transform.DOLocalRotate(new Vector3(0, 0, hoverRotationAngle), hoverDuration)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .OnComplete(() => StartScaleEffect());
        }
    }

    private void StartScaleEffect()
    {
        KillTweens();
        transform.rotation = Quaternion.identity;
        _scaleTween = transform.DOScale(_originalScale * hoverScaleMultiplier, scaleDuration)
            .SetEase(Ease.OutSine)
            .OnStart(() => _hoverTween = null);
    }

    public void StopHoverEffect()
    {
        KillTweens();
        transform.rotation = Quaternion.identity;
        transform.localScale = _originalScale;
    }

    private void KillTweens()
    {
        _hoverTween?.Kill();
        _scaleTween?.Kill();
        _dragTween?.Kill();
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Mathf.Abs(_mainCamera.transform.position.z - transform.position.z);
        return _mainCamera.ScreenToWorldPoint(mousePosition);
    }
}
