using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public TMP_Text AreaOnePlayerScore;
    public TMP_Text AreaTwoPlayerScore;
    public TMP_Text AreaThreePlayerScore;
    public TMP_Text AreaOneOpponentScore;
    public TMP_Text AreaTwoOpponentScore;
    public TMP_Text AreaThreeOpponentScore;
    private Dictionary<int, int> playerScores = new Dictionary<int, int>();
    private Dictionary<int, int> opponentScores = new Dictionary<int, int>();
    private HashSet<int> manuallySetZones = new HashSet<int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        for (int i = 1; i <= 3; i++)
        {
            playerScores[i] = 0;
            opponentScores[i] = 0;
        }
    }
    public void CalculatePower(List<PlayArea> playerAreas, List<PlayArea> opponentAreas)
    {
        ResetScores(playerScores);
        ResetScores(opponentScores);

        foreach (PlayArea area in playerAreas)
        {
            if (area.PlacedAmount() > 0)
            {
                playerScores[area.Index] += area.PlacedCardsPower();
            }
        }
        foreach (PlayArea area in opponentAreas)
        {
            if (area.PlacedAmount() > 0)
            {
                opponentScores[area.Index] += area.PlacedCardsPower();
            }
        }

        UpdateUI();
    }


    private void UpdateUI()
    {
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
            playerScores[index] = amount;
        }
        else
        {
            opponentScores[index] = amount;
        }
        manuallySetZones.Add(index);
    }
}

