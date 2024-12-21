using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingerEffect : ICardEffect
{
    int powerAmount = 2;
    public void ApplyEffect(Card card)
    {
        //Gain +3 power if played in right area — On reveal
        if (card.PlacedOpponentArea == null)
        {
            //player
            if (card.PlacedArea.Index == 3 || card.PlacedArea.Index == 1)
                card.IncreasePower(powerAmount);
        }
        else
        {
            //opponent
            if (card.PlacedOpponentArea.Index == 3 || card.PlacedOpponentArea.Index == 1)
                card.IncreasePower(powerAmount);
        }
    }
}
