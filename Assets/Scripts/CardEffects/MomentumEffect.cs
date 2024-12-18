using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomentumEffect : ICardEffect
{
    public void ApplyEffect(Card card)
    {
        //Gain +3 power if you play a card here next turn — Ongoing
        if(card.PlacedOpponentArea != null)
        {
            card.PlacedOpponentArea.SaveMomentumEffect(card);
        }
        else if(card.PlacedOpponentArea == null && card.PlacedArea != null)
        {
            card.PlacedArea.SaveMomentumEffect(card);
        }
    }
}
