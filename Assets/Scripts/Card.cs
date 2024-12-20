using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

public class Card : MonoBehaviour
{
    public IBattlegroundEffect CardAbility { get; private set; }
    public CardClass CardClassType;
    public CardTrigger CardTriggerType;
    public CardEffect CardEffectType;
    private ICardEffect _cardEffect;
    public CardDisplay CardDisplay { get; private set; }
    private GameObject _cardVisual;
    private bool _playedUpdated = false;

    public GameObject CardTooltipPrefab;
    private GameObject _currentTooltip;
    private Vector3 _tooltipOffset = new Vector3(1f, 0, 0);

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
    private bool _roundPlaying = false;
    public GameObject Shadow;
    private Collider2D _collider;
    public bool HasReceivedBoost { get; set; } = false;

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
    public bool PlacedOnArea
    {
        get { return _placedOnArea; }
        set { _placedOnArea = value; }
    }
    private PlayArea _placedOpponentArea;
    public PlayArea PlacedOpponentArea => _placedOpponentArea;
    public PlayArea PlacedArea => _placedArea;
    private bool _played=false;
    public bool Played
    {
        get { return _played; }
        set { _played = value; }
    }
    private void OnEnable()
    {
        RoundManager.OnRoundStarted += UpdatePlayed;
        RoundManager.OnRoundEnded += RoundEnd;
    }
    private void OnDisable()
    {
        RoundManager.OnRoundStarted -= UpdatePlayed;
        RoundManager.OnRoundEnded -= RoundEnd;
    }
    private void Awake()
    {
        _cardVisual= transform.Find("CardVisual").gameObject;
    }
    private void Start()
    {
        InitializeCard();
        StartIdleMovement();
    }
    private void InitializeCard()
    {
        _originalScale = _cardVisual.transform.localScale;
        _shadowOriginalLocalPosition = Shadow != null ? Shadow.transform.localPosition : Vector3.zero;
        _hoverScale = _originalScale * 1.05f;
        _movingScale = _originalScale * 1.05f * 1.10f;
        _mainCamera = Camera.main;
        _collider = GetComponent<BoxCollider2D>();
        CardDisplay = GetComponent<CardDisplay>();
        _cardEffect = CardEffectFactory.GetCardEffect(CardEffectType);
    }

    private void UpdatePlayed()
    {
        if (_playedUpdated) return;
        if (_placedOnArea && PlacedOpponentArea == null)
        {
            Played = true;
            _playedUpdated = true;
        }
        _roundPlaying = true;
    }

    public void TriggerCardEffect()
    {
        if(CardEffectType!=CardEffect.None)
            _cardEffect.ApplyEffect(this);
    }
    public void SetOpponentArea(PlayArea area)
    {
        _placedOpponentArea = area;
    }

    public void SetPlayerArea(PlayArea area)
    {
        _placedArea = area;
    }

    public void SetOriginalPosition(Vector3 position)
    {
        _originalPosition = position;
    }

    private void ShowCardTooltip()
    {
        if (_currentTooltip != null) return;

        _currentTooltip = Instantiate(CardTooltipPrefab, _cardVisual.transform);
        _currentTooltip.transform.localPosition = _tooltipOffset;

        TextMeshProUGUI tooltipText = _currentTooltip.GetComponentInChildren<TextMeshProUGUI>();
        if (tooltipText != null)
        {
            string description = CardEffectDescriptions.GetDescription(CardEffectType);
            tooltipText.text = $"{AddSpacesToPascalCase(CardTriggerType.ToString())}\n\n" +
                   $"{description}";
        }
    }
    private void HideCardTooltip()
    {
        if (_currentTooltip != null)
        {
            Destroy(_currentTooltip);
            _currentTooltip = null;
        }
    }
    public string AddSpacesToPascalCase(string text)
    {
        return Regex.Replace(text, "(\\B[A-Z])", " $1");
    }
    private void OnMouseDown()
    {
        if (Played || !EnergyManager.Instance.CheckIfMovable(GetComponent<CardDisplay>().Energy, this) || _roundPlaying) return;
        AdjustChildSortingOrder(2);
        _offset = transform.position - GetMouseWorldPosition();
        _isDragging = true;
        KillTweens();
        _cardVisual.transform.DOScale(_movingScale, scaleDuration).SetEase(Ease.OutSine);
    }

    private void OnMouseUp()
    {
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

    private void RoundEnd()
    {
        _roundPlaying = false;
        KillTweens();
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
