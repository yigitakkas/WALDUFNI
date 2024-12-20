using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

public partial class Card : MonoBehaviour
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
    private bool _played = false;
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
        _cardVisual = transform.Find("CardVisual").gameObject;
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
}
