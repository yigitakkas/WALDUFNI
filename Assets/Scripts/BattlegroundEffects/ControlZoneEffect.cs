using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlZoneEffect : IBattlegroundEffect
{
    public void ApplyEffect(Battleground battleground)
    {

        PlayArea playerArea = RoundManager.Instance.GetAreaWithIndex(battleground.Index, true);
        PlayArea opponentArea = RoundManager.Instance.GetAreaWithIndex(battleground.Index, false);

        int playerCardCount = playerArea.PlacedCards.Count;
        int opponentCardCount = opponentArea.PlacedCards.Count;

        if(playerCardCount > opponentCardCount)
        {
            ScoreManager.Instance.SetScoreWithIndex(battleground.Index, player: true, amount: 100);
            ScoreManager.Instance.SetScoreWithIndex(battleground.Index, player: false, amount: 0);
        } 
        else if(opponentCardCount > playerCardCount)
        {
            ScoreManager.Instance.SetScoreWithIndex(battleground.Index, player: true, amount: 0);
            ScoreManager.Instance.SetScoreWithIndex(battleground.Index, player: false, amount: 100);
        }
        else
        {
            ScoreManager.Instance.SetScoreWithIndex(battleground.Index, player: true, amount: 100);
            ScoreManager.Instance.SetScoreWithIndex(battleground.Index, player: false, amount: 100);
        }
    }
}
