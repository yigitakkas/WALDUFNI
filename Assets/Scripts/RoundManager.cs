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
        Instance = this;
        SetRound(1);
    }
    private void Start()
    {
        FindAreas();
        _opponentPlayAreas = Opponent.Instance.ReturnOpponentAreas();
    }
    public void SetRound(int round)
    {
        CurrentRound = round;
    }
    private void FindAreas()
    {
        _playerPlayAreas.Add(transform.Find("PlayerArea1").GetComponent<PlayArea>());
        _playerPlayAreas.Add(transform.Find("PlayerArea2").GetComponent<PlayArea>());
        _playerPlayAreas.Add(transform.Find("PlayerArea3").GetComponent<PlayArea>());
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
            ScoreManager.CalculatePower(_playerPlayAreas, _opponentPlayAreas, ref _playerScore, ref _opponentScore);
            UpdateScoreUI();
            CurrentRound++;
            OnRoundEnded?.Invoke();
        }
        else
        {
            Debug.LogError("No cards placed on any area");
        }
        _placedCardsAmount = 0;
    }

    private void UpdateScoreUI()
    {
        PlayerScoreText.text = $"PLAYER: {_playerScore}";
        OpponentScoreText.text = $"OPP: {_opponentScore}";
    }
}

public static class ScoreManager
{
    public static void CalculatePower(List<PlayArea> playerAreas, List<PlayArea> opponentAreas, ref int playerScore, ref int opponentScore)
    {
        int playerFirstZone = 0;
        int playerSecondZone = 0;
        int playerThirdZone = 0;
        int opponentFirstZone = 0;
        int opponentSecondZone = 0;
        int opponentThirdZone = 0;

        foreach (PlayArea area in playerAreas)
        {
            if (area.Index == 1 && area.PlacedAmount() != 0)
            {
                playerFirstZone += area.PlacedCardsPower();
            }
            if (area.Index == 2 && area.PlacedAmount() != 0)
            {
                playerSecondZone += area.PlacedCardsPower();
            }
            if (area.Index == 3 && area.PlacedAmount() != 0)
            {
                playerThirdZone += area.PlacedCardsPower();
            }
        }
        foreach (PlayArea area in opponentAreas)
        {
            if (area.Index == 1 && area.PlacedAmount() != 0)
            {
                opponentFirstZone += area.PlacedCardsPower();
            }
            if (area.Index == 2 && area.PlacedAmount() != 0)
            {
                opponentSecondZone += area.PlacedCardsPower();
            }
            if (area.Index == 3 && area.PlacedAmount() != 0)
            {
                opponentThirdZone += area.PlacedCardsPower();
            }
        }
        Debug.Log($"playerFirstZone: {playerFirstZone}, playerSecondZone: {playerSecondZone}, playerThirdZone: {playerThirdZone}, " +
                  $"opponentFirstZone: {opponentFirstZone}, opponentSecondZone: {opponentSecondZone}, opponentThirdZone: {opponentThirdZone}");

        playerScore += (playerFirstZone > opponentFirstZone) ? 1 : 0;
        opponentScore += (playerFirstZone < opponentFirstZone) ? 1 : 0;

        playerScore += (playerSecondZone > opponentSecondZone) ? 1 : 0;
        opponentScore += (playerSecondZone < opponentSecondZone) ? 1 : 0;

        playerScore += (playerThirdZone > opponentThirdZone) ? 1 : 0;
        opponentScore += (playerThirdZone < opponentThirdZone) ? 1 : 0;
    }
}
