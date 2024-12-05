using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoundManager : MonoBehaviour
{
    [SerializeField]
    public List<PlayArea> PlayAreas = new List<PlayArea>();
    private int _placedCardsAmount = 0;

    [SerializeField]
    private List<PlayArea> _opponentPlayAreas = new List<PlayArea>();

    private int _playerScore=0;
    private int _opponentScore=0;

    public TMP_Text PlayerScoreText;
    public TMP_Text OpponentScoreText;


    private void Start()
    {
        _opponentPlayAreas = Opponent.Instance.ReturnOpponentAreas();
    }
    public void StartRound()
    {
        foreach(PlayArea area in PlayAreas)
        {
            _placedCardsAmount += area.PlacedAmount();
        }
        if (_placedCardsAmount > 0)
        {
            Opponent.Instance.PlayHand();
        } else
        {
            Debug.LogError("No cards placed on any area");
        }
        CalculatePower();
        _placedCardsAmount = 0;
    }

    private void CalculatePower()
    {
        int playerFirstZone = 0;
        int playerSecondZone = 0;
        int playerThirdZone = 0;
        int opponentFirstZone = 0;
        int opponentSecondZone = 0;
        int opponentThirdZone = 0;

        foreach (PlayArea area in PlayAreas)
        {
            if(area.Index==1 && area.PlacedAmount() != 0)
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
        foreach (PlayArea area in _opponentPlayAreas)
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

        _playerScore += (playerFirstZone > opponentFirstZone) ? 1 : 0;
        _opponentScore += (playerFirstZone < opponentFirstZone) ? 1 : 0;

        _playerScore += (playerSecondZone > opponentSecondZone) ? 1 : 0;
        _opponentScore += (playerSecondZone < opponentSecondZone) ? 1 : 0;

        _playerScore += (playerThirdZone > opponentThirdZone) ? 1 : 0;
        _opponentScore += (playerThirdZone < opponentThirdZone) ? 1 : 0;

        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        PlayerScoreText.text = $"PLAYER: {_playerScore}";
        OpponentScoreText.text = $"OPP: {_opponentScore}";
    }
}
