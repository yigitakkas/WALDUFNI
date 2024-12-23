using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject PopUpPanel;
    public TMP_Text WonText;
    public GameObject BlockerPanel;

    public Button PlayButton;

    public TMP_Text AreaOnePlayerScore;
    public TMP_Text AreaTwoPlayerScore;
    public TMP_Text AreaThreePlayerScore;
    public TMP_Text AreaOneOpponentScore;
    public TMP_Text AreaTwoOpponentScore;
    public TMP_Text AreaThreeOpponentScore;

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
    }
    private void OnEnable()
    {
        RoundManager.OnRoundStarted += DeactivateButton;
        RoundManager.OnRoundEnded += ActivateButton;
    }

    private void OnDisable()
    {
        RoundManager.OnRoundStarted -= DeactivateButton;
        RoundManager.OnRoundEnded -= ActivateButton;
    }
    public void ShowPopup(string message, bool isPlayerWinner)
    {
        StartCoroutine(GiveEffectToScores());

        BlockerPanel.SetActive(true);
        PopUpPanel.SetActive(true);

        if (isPlayerWinner)
        {
            WonText.color = Color.blue;
        }
        else
        {
            WonText.color = Color.red;
        }

        WonText.text = message;
    }

    public void HidePopup()
    {
        WonText.text = "";
        WonText.color = Color.white;
        BlockerPanel.SetActive(false);
        PopUpPanel.SetActive(false);
    }

    private void ActivateButton()
    {
        PlayButton.interactable = true;
    }
    private void DeactivateButton()
    {
        PlayButton.interactable = false;
    }
    public void UpdateUI()
    {
        Dictionary<int, int> playerScores = ScoreManager.Instance.PlayerScores;
        Dictionary<int, int> opponentScores = ScoreManager.Instance.OpponentScores;

        AreaOnePlayerScore.text = playerScores[1].ToString();
        AreaTwoPlayerScore.text = playerScores[2].ToString();
        AreaThreePlayerScore.text = playerScores[3].ToString();

        AreaOneOpponentScore.text = opponentScores[1].ToString();
        AreaTwoOpponentScore.text = opponentScores[2].ToString();
        AreaThreeOpponentScore.text = opponentScores[3].ToString();

        CompareAndSetTextColor(AreaOnePlayerScore, AreaOneOpponentScore, playerScores[1], opponentScores[1]);
        CompareAndSetTextColor(AreaTwoPlayerScore, AreaTwoOpponentScore, playerScores[2], opponentScores[2]);
        CompareAndSetTextColor(AreaThreePlayerScore, AreaThreeOpponentScore, playerScores[3], opponentScores[3]);
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

    IEnumerator GiveEffectToScores()
    {
        //effect code
        yield return new WaitForSeconds(2f);
    }
}
