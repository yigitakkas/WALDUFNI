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

    private int MaxRound = 5;

    public int CurrentRound { get; private set; }
    private int _placedCardsAmount = 0;

    [SerializeField]
    private List<PlayArea> _playerPlayAreas = new List<PlayArea>();
    public List<PlayArea> PlayerPlayAreas
    {
        get => _playerPlayAreas; 
        private set => _playerPlayAreas = value; 
    }


    [SerializeField]
    private List<PlayArea> _opponentPlayAreas = new List<PlayArea>();
    public List<PlayArea> OpponentPlayAreas
    {
        get => _opponentPlayAreas;
        private set => _opponentPlayAreas = value;
    }

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
        ScoreManager.Instance.CalculatePower(_playerPlayAreas, _opponentPlayAreas);
    }
    public void SetRound(int round)
    {
        CurrentRound = round;
    }
    private void FindAreas()
    {
        foreach (PlayArea area in GetComponentsInChildren<PlayArea>())
        {
            if (area.CompareTag("PlayerArea"))
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

            BattlegroundManager.Instance.ApplyBattlegroundEffects();
            ScoreManager.Instance.CalculatePower(_playerPlayAreas, _opponentPlayAreas);

            CurrentRound++;
            OnRoundEnded?.Invoke();
        }
        else
        {
            Debug.LogError("No cards placed on any area");
        }
        _placedCardsAmount = 0;
    }

    public PlayArea GetAreaWithIndex(int index, bool player)
    {
        if(player)
        {
            return PlayerPlayAreas[index-1];
        }
        else
        {
            return OpponentPlayAreas[index-1];
        }
    }
}