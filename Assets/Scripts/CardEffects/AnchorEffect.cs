using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorEffect : ICardEffect
{
    int powerAmount = 3;
    public void ApplyEffect(Card card)
    {
        //Gain +3 power if played in middle area — On reveal
        if (card.PlacedOpponentArea == null)
        {
            //player
            if (card.PlacedArea.Index == 2)
                card.IncreasePower(powerAmount);
        }
        else
        {
            //opponent
            if (card.PlacedOpponentArea.Index == 2)
                card.IncreasePower(powerAmount);
        }
    }
}
