using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    public GameObject PopUpPanel;
    public TMP_Text WonText;
    public Button NextLevelButton;
    public Button TryAgainButton;
    public GameObject BlockerPanel;
    public Button PlayButton;

    private void OnEnable()
    {
        RoundManager.OnRoundStarted += DeactivateButton;
        RoundManager.OnRoundEnded += ActivateButton;
        RoundManager.GameEnded += DefineWinner;
    }

    private void OnDisable()
    {
        RoundManager.OnRoundStarted -= DeactivateButton;
        RoundManager.OnRoundEnded -= ActivateButton;
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
        ShowPopup("PLAYER WON!");
    }

    private void OpponentWon()
    {
        ShowPopup("OPPONENT WON!");
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

    private void ShowPopup(string Message)
    {
        BlockerPanel.SetActive(true);
        PopUpPanel.SetActive(true);
        if (Message == "PLAYER WON!")
        {
            WonText.color = Color.blue;
            NextLevelButton.gameObject.SetActive(true);
        }
        else
        {
            WonText.color = Color.red;
            TryAgainButton.gameObject.SetActive(true);
        }
        WonText.text = Message;
    }

    private void HidePopup()
    {
        WonText.text = "";
        WonText.color = Color.white;
        BlockerPanel.SetActive(false);
        PopUpPanel.SetActive(false);
        NextLevelButton.gameObject.SetActive(false);
        TryAgainButton.gameObject.SetActive(false);
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void TryAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ActivateButton()
    {
        PlayButton.interactable = true;
    }
    private void DeactivateButton()
    {
        PlayButton.interactable = false;
    }
}

