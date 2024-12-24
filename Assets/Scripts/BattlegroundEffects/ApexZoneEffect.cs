using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ApexZoneEffect : IBattlegroundEffect
{
    public void ApplyEffect(Battleground battleground)
    {
        UpdateAllCards(battleground);

        List<Card> strongestCards = GetStrongestCards(battleground);

        BoostStrongestCards(strongestCards, 3,battleground);
    }

    private void UpdateAllCards(Battleground battleground)
    {
        PlayArea player = RoundManager.Instance.GetAreaWithIndex(battleground.Index, true);
        PlayArea opponent = RoundManager.Instance.GetAreaWithIndex(battleground.Index, false);

        battleground.AllCards.UnionWith(player.PlacedCards);
        battleground.AllCards.UnionWith(opponent.PlacedCards);
        battleground.AllCardsList = new List<Card>(battleground.AllCards); // Listeyi Inspector'da görüntüleme
    }

    private List<Card> GetStrongestCards(Battleground battleground)
    {
        List<Card> strongestCards = new List<Card>();

        if (battleground.AllCards == null || battleground.AllCards.Count == 0)
        {
            return strongestCards;
        }

        int maxPower = int.MinValue;

        foreach (Card card in battleground.AllCards)
        {
            int cardStrength = card.Power;

            if (cardStrength > maxPower)
            {
                maxPower = cardStrength;
                strongestCards.Clear();
                strongestCards.Add(card);
            }
            else if (cardStrength == maxPower)
            {
                strongestCards.Add(card);
            }
        }

        return strongestCards;
    }

    private void BoostStrongestCards(List<Card> cards, int boostAmount, Battleground battleground)
    {
        foreach (Card card in cards)
        {
            if (!card.HasReceivedBoost)
            {
                EffectManager.Instance.PlayBoostEffect(
                    battleground.transform.position,
                    card.transform.position,
                    0.5f,
                    () =>
                    {
                        card.IncreasePower(boostAmount);
                        card.HasReceivedBoost = true;
                    }
                );
            }
        }
    }
}
