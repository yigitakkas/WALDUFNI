using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance;

    public static event Action OnRoundStarted;
    public static event Action OnRoundEnded;

    public int CurrentRound { get; private set; }

    [SerializeField]
    private List<PlayArea> _playerPlayAreas = new List<PlayArea>();
    private int _placedCardsAmount = 0;

    [SerializeField]
    private List<PlayArea> _opponentPlayAreas = new List<PlayArea>();

    private int _playerScore = 0;
    private int _opponentScore = 0;

    public TMP_Text PlayerScoreText;
    public TMP_Text OpponentScoreText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        SetRound(1);
        FindAreas();
    }

    private void Start()
    {
        _opponentPlayAreas = OpponentManager.Instance.ReturnOpponentAreas();
        ScoreManager.Instance.CalculatePower(_playerPlayAreas, _opponentPlayAreas, ref _playerScore, ref _opponentScore);
    }
    public void SetRound(int round)
    {
        CurrentRound = round;
    }
    private void FindAreas()
    {
        foreach (PlayArea area in GetComponentsInChildren<PlayArea>())
        {
            if (area.CompareTag("PlayerArea")) // Alanlarý bir tag ile filtreleyebilirsiniz
            {
                _playerPlayAreas.Add(area);
            }
        }
    }
    public void StartRound()
    {
        foreach (PlayArea area in _playerPlayAreas)
        {
            _placedCardsAmount += area.PlacedAmount();
        }
        if (_placedCardsAmount > 0)
        {
            OnRoundStarted?.Invoke();

            ScoreManager.Instance.CalculatePower(_playerPlayAreas, _opponentPlayAreas, ref _playerScore, ref _opponentScore);
            CurrentRound++;

            OnRoundEnded?.Invoke();
        }
        else
        {
            Debug.LogError("No cards placed on any area");
        }
        _placedCardsAmount = 0;
    }

}