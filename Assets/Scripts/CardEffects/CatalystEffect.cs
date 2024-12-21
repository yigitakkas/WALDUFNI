using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatalystEffect : ICardEffect
{
    public void ApplyEffect(Card card)
    {
        PlayArea area = null;
        //Gain +2 power for every card placed here — Ongoing
        if (card.PlacedOpponentArea != null)
        {
            area = card.PlacedOpponentArea;
        }
        else if (card.PlacedOpponentArea == null && card.PlacedArea != null)
        {
            area = card.PlacedArea;
        }
        int amount = area.PlacedAmount() - 1;
        card.IncreasePower(2 * amount);
    }
}
