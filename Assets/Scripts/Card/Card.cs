using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

public partial class Card : MonoBehaviour
{
    // Temel Özellikler ve Referanslar
    public IBattlegroundEffect CardAbility { get; private set; }
    public CardClass CardClassType;
    public CardTrigger CardTriggerType;
    public CardEffect CardEffectType;
    private ICardEffect _cardEffect;
    private Color _originalColor;
    private BoxCollider2D _collider;

    // Kart Durumlarý
    private bool _playedUpdated = false;
    private bool _isDragging = false;
    private bool _isHovered = false;
    private bool _roundPlaying = false;
    public bool HasReceivedBoost { get; set; } = false;
    private bool _placedOnArea = false;
    private bool _played = false;
    public bool IsDragging => _isDragging;

    // Kart Alanlarý
    private PlayArea _currentPlayArea;
    private PlayArea _placedArea;
    private PlayArea _placedOpponentArea;
    public PlayArea PlacedOpponentArea => _placedOpponentArea;
    public PlayArea PlacedArea => _placedArea;
    public bool PlacedOnArea
    {
        get { return _placedOnArea; }
        set { _placedOnArea = value; }
    }
    public bool Played
    {
        get { return _played; }
        set { _played = value; }
    }

    // Kart Görsel ve UI Elemanlarý
    private GameObject _cardVisual;
    public GameObject Shadow;
    public GameObject CardTooltipPrefab;
    private GameObject _currentTooltip;
    private GameObject _cardNameObject;
    private GameObject _powerObject;
    private GameObject _energyObject;
    private GameObject _overlayObject;
    private TextMeshProUGUI _cardNameText;
    private TextMeshProUGUI _powerText;
    private TextMeshProUGUI _energyText;

    // Kart Özellikleri
    [Header("Card Attributes")]
    public string CharacterName;
    public int Energy;
    public int Power;

    // UI ve Görsel Efekt Ayarlarý
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

    // Kamera ve Konum Deðiþkenleri
    private Camera _mainCamera;
    private Vector3 _originalPosition;
    private Vector3 _offset;
    private Vector3 _tooltipOffset = new Vector3(1f, 0, 0);

    // Tween ve Hareket Deðiþkenleri
    private Tween _hoverTween;
    private Tween _scaleTween;
    private Tween _idleTween;
    private Tween _shadowMoveTween;
    private Vector3 _originalScale;
    private Vector3 _shadowOriginalLocalPosition;
    private Vector3 _hoverScale;
    private Vector3 _movingScale;

    private void OnEnable()
    {
        RoundManager.OnRoundStarted += UpdatePlayed;
        RoundManager.OnRoundEnded += RoundEnd;
        RoundManager.GameEnded += DisableCollider;
    }

    private void OnDisable()
    {
        RoundManager.OnRoundStarted -= UpdatePlayed;
        RoundManager.OnRoundEnded -= RoundEnd;
        RoundManager.GameEnded -= DisableCollider;
    }

    private void Awake()
    {
        _cardVisual = transform.Find("CardVisual").gameObject;
        CardDisplayAwake();
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
        _cardEffect = CardEffectFactory.GetCardEffect(CardEffectType);
    }

    public void TriggerCardEffect()
    {
        if (CardEffectType != CardEffect.None && CardEffectType != CardEffect.Pioneer)
            _cardEffect.ApplyEffect(this);
    }

    private void DisableCollider()
    {
        _collider.enabled = false;
    }
}
