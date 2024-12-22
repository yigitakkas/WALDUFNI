using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public TMP_Text AreaOnePlayerScore;
    public TMP_Text AreaTwoPlayerScore;
    public TMP_Text AreaThreePlayerScore;
    public TMP_Text AreaOneOpponentScore;
    public TMP_Text AreaTwoOpponentScore;
    public TMP_Text AreaThreeOpponentScore;
    private Dictionary<int, int> _playerScores = new Dictionary<int, int>();
    private Dictionary<int, int> _opponentScores = new Dictionary<int, int>();
    private HashSet<int> manuallySetZones = new HashSet<int>();

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
        UpdateUI();
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


    public void UpdateUI()
    {
        AreaOnePlayerScore.text = _playerScores[1].ToString();
        AreaTwoPlayerScore.text = _playerScores[2].ToString();
        AreaThreePlayerScore.text = _playerScores[3].ToString();

        AreaOneOpponentScore.text = _opponentScores[1].ToString();
        AreaTwoOpponentScore.text = _opponentScores[2].ToString();
        AreaThreeOpponentScore.text = _opponentScores[3].ToString();

        CompareAndSetTextColor(AreaOnePlayerScore, AreaOneOpponentScore, _playerScores[1], _opponentScores[1]);
        CompareAndSetTextColor(AreaTwoPlayerScore, AreaTwoOpponentScore, _playerScores[2], _opponentScores[2]);
        CompareAndSetTextColor(AreaThreePlayerScore, AreaThreeOpponentScore, _playerScores[3], _opponentScores[3]);
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


    private void CompareAndSetTextColor(TMP_Text playerText, TMP_Text opponentText, int playerScore, int opponentScore)
    {
        if (playerScore > opponentScore)
        {
            playerText.color = Color.green; 
            opponentText.color = Color.red;
        }
        else if (playerScore < opponentScore)
        {
            playerText.color = Color.red;
            opponentText.color = Color.green;
        }
        else
        {
            playerText.color = Color.yellow;
            opponentText.color = Color.yellow;
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
        UIManager.Instance.ShowPopup("PLAYER WON!", isPlayerWinner:true);
    }

    private void OpponentWon()
    {
        UIManager.Instance.ShowPopup("OPPONENT WON!", isPlayerWinner: false);
    }

    private void DefineWinnerInDraw()
    {
        int playerTotalCardPower = 0;

        foreach (var score in _playerScores.Values)
        {
            playerTotalCardPower += score;
        }

        int opponentTotalCardPower = 0;

        foreach (var score in _playerScores.Values)
        {
            opponentTotalCardPower += score;
        }

        if (playerTotalCardPower > opponentTotalCardPower)
            PlayerWon();
        else if (playerTotalCardPower <= opponentTotalCardPower)
            OpponentWon();
    }
}

