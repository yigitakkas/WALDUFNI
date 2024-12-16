using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePowerBoostEffect : IBattlegroundEffect
{
    private HashSet<Card> boostedCards = new HashSet<Card>();

    protected abstract int PowerBoostAmount { get; }

    public void ApplyEffect(Battleground battleground)
    {
        battleground.UpdateAllCards();

        foreach (Card card in battleground.AllCards)
        {
            if (!boostedCards.Contains(card))
            {
                CardDisplay cardDisplay = card.GetComponent<CardDisplay>();
                cardDisplay.IncreasePower(PowerBoostAmount);
                boostedCards.Add(card);

                Debug.Log($"{GetType().Name}: {card.name} gained +{PowerBoostAmount} Power. New Power: {cardDisplay.Power}");
            }
        }
    }
}
