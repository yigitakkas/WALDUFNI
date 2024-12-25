using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    private Dictionary<int, int> _playerScores = new Dictionary<int, int>();
    private Dictionary<int, int> _opponentScores = new Dictionary<int, int>();
    private HashSet<int> manuallySetZones = new HashSet<int>();
    public Dictionary<int, int> PlayerScores
    {
        get => _playerScores;
    }
    public Dictionary<int, int> OpponentScores
    {
        get => _opponentScores;
    }

    private void OnEnable()
    {
        RoundManager.GameEnded += DefineWinner;
    }

    private void OnDisable()
    {
        RoundManager.GameEnded -= DefineWinner;
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        for (int i = 1; i <= 3; i++)
        {
            _playerScores[i] = 0;
            _opponentScores[i] = 0;
        }
        UIManager.Instance.UpdateUI();
    }
    public void CalculatePower(List<PlayArea> playerAreas, List<PlayArea> opponentAreas)
    {
        ResetScores(_playerScores);
        ResetScores(_opponentScores);

        foreach (PlayArea area in playerAreas)
        {
            if (area.PlacedAmount() > 0)
            {
                _playerScores[area.Index] += area.PlacedCardsPower();
            }
        }
        foreach (PlayArea area in opponentAreas)
        {
            if (area.PlacedAmount() > 0)
            {
                _opponentScores[area.Index] += area.PlacedCardsPower();
            }
        }
    }



    private void ResetScores(Dictionary<int, int> scores)
    {
        foreach (int index in new List<int>(scores.Keys))
        {
            if (!manuallySetZones.Contains(index))
            {
                scores[index] = 0;
            }
        }
    }

    public void SetScoreWithIndex(int index, bool player, int amount)
    {
        if (player)
        {
            _playerScores[index] = amount;
        }
        else
        {
            _opponentScores[index] = amount;
        }
        manuallySetZones.Add(index);
    }

    private void DefineWinner()
    {
        int playerWonAmount = 0;
        int opponentWonAmount = 0;

        CompareScores(_playerScores[1], _opponentScores[1], ref playerWonAmount, ref opponentWonAmount);
        CompareScores(_playerScores[2], _opponentScores[2], ref playerWonAmount, ref opponentWonAmount);
        CompareScores(_playerScores[3], _opponentScores[3], ref playerWonAmount, ref opponentWonAmount);

        if (playerWonAmount > opponentWonAmount)
            PlayerWon();
        else if (playerWonAmount < opponentWonAmount)
            OpponentWon();
        else if (playerWonAmount == opponentWonAmount)
            DefineWinnerInDraw();
    }

    private void CompareScores(int playerScore, int opponentScore, ref int playerWonAmount, ref int opponentWonAmount)
    {
        if (playerScore > opponentScore)
        {
            playerWonAmount++;
        }
        else if (playerScore < opponentScore)
        {
            opponentWonAmount++;
        }
    }

    private void PlayerWon()
    {
        StartCoroutine(UIManager.Instance.ShowPopup("PLAYER WON!", isPlayerWinner: true));
    }

    private void OpponentWon()
    {
        StartCoroutine(UIManager.Instance.ShowPopup("OPPONENT WON!", isPlayerWinner: false));
    }

    private void DefineWinnerInDraw()
    {
        int playerTotalCardPower = 0;

        foreach (var score in _playerScores.Values)
        {
            playerTotalCardPower += score;
        }

        int opponentTotalCardPower = 0;

        foreach (var score in _opponentScores.Values)
        {
            opponentTotalCardPower += score;
        }

        if (playerTotalCardPower >= opponentTotalCardPower)
            PlayerWon();
        else if (playerTotalCardPower < opponentTotalCardPower)
            OpponentWon();
    }

    public int IsPlayerWinningZone(int index)
    {
        if (_playerScores[index] > _opponentScores[index])
            return 1;
        else if (_playerScores[index] < _opponentScores[index])
            return 2;
        else return 0;
    }
}

