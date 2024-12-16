using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApexZoneEffect : IBattlegroundEffect
{
    public void ApplyEffect(Battleground battleground)
    {
        // Kartlar� g�ncelle
        UpdateAllCards(battleground);

        // En g��l� kartlar� bul
        List<Card> strongestCards = GetStrongestCards(battleground);

        // G��lendirme uygula
        BoostStrongestCards(strongestCards, 3);
    }

    private void UpdateAllCards(Battleground battleground)
    {
        PlayArea player = RoundManager.Instance.GetAreaWithIndex(battleground.Index, true);
        PlayArea opponent = RoundManager.Instance.GetAreaWithIndex(battleground.Index, false);

        battleground.AllCards.UnionWith(player.PlacedCards);
        battleground.AllCards.UnionWith(opponent.PlacedCards);
        battleground.AllCardsList = new List<Card>(battleground.AllCards); // Listeyi Inspector'da g�r�nt�leme
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
            int cardStrength = card.GetComponent<CardDisplay>().Power;

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

    private void BoostStrongestCards(List<Card> cards, int boostAmount)
    {
        foreach (Card card in cards)
        {
            if (!card.HasReceivedBoost)
            {
                CardDisplay cardDisplay = card.GetComponent<CardDisplay>();
                cardDisplay.IncreasePower(boostAmount);
                card.HasReceivedBoost = true;
            }
        }
    }
}
