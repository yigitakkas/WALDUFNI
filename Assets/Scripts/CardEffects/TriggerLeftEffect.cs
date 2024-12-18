using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerLeftEffect : ICardEffect
{
    int powerAmount = 3;
    public void ApplyEffect(Card card)
    {
        //Gain +3 power if played in left area — On reveal
        if (card.PlacedOpponentArea == null)
        {
            //player
            if (card.PlacedArea.Index == 1)
                card.CardDisplay.IncreasePower(powerAmount);
        } else
        {
            //opponent
            if (card.PlacedOpponentArea.Index == 1)
                card.CardDisplay.IncreasePower(powerAmount);
        }
    }
}
