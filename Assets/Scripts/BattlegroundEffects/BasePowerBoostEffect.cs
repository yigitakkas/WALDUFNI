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
                EffectManager.Instance.PlayBoostEffect(
                    battleground.transform.position,
                    card.transform.position,
                    0.5f,
                    () =>
                    {
                        card.IncreasePower(PowerBoostAmount);
                        boostedCards.Add(card);
                    }
                );
            }
        }

    }
}
