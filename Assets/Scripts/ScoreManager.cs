using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public TMP_Text PlayerScoreText;
    public TMP_Text OpponentScoreText;

    private void Awake()
    {
        Instance = this;
    }
    public void CalculatePower(List<PlayArea> playerAreas, List<PlayArea> opponentAreas, ref int playerScore, ref int opponentScore)
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


        PlayerScoreText.text = $"PLAYER: {playerScore}";
        OpponentScoreText.text = $"OPP: {opponentScore}";
    }
}

