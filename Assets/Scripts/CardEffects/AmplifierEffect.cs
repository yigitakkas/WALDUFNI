using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmplifierEffect : ICardEffect
{
    public void ApplyEffect(Card card)
    {
        //Doubles the total power of the area — On reveal
        if (card.PlacedOpponentArea != null)
        {
            PlayArea area = card.PlacedOpponentArea;
            area.SetAmplified();
        }
        else if (card.PlacedOpponentArea == null && card.PlacedArea != null)
        {
            PlayArea area = card.PlacedArea;
            area.SetAmplified();
        }
    }
}
