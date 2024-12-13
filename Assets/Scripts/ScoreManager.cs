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

    private void Awake()
    {
        Instance = this;
    }
    public void CalculatePower(List<PlayArea> playerAreas, List<PlayArea> opponentAreas)
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

        AreaOnePlayerScore.text = playerFirstZone.ToString();
        AreaTwoPlayerScore.text = playerSecondZone.ToString();
        AreaThreePlayerScore.text = playerThirdZone.ToString();

        AreaOneOpponentScore.text = opponentFirstZone.ToString();
        AreaTwoOpponentScore.text = opponentSecondZone.ToString();
        AreaThreeOpponentScore.text = opponentThirdZone.ToString();

        CompareAndSetTextColor(AreaOnePlayerScore, AreaOneOpponentScore, playerFirstZone, opponentFirstZone);
        CompareAndSetTextColor(AreaTwoPlayerScore, AreaTwoOpponentScore, playerSecondZone, opponentSecondZone);
        CompareAndSetTextColor(AreaThreePlayerScore, AreaThreeOpponentScore, playerThirdZone, opponentThirdZone);
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
}

