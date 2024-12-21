using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EchoEffect : ICardEffect
{
    public void ApplyEffect(Card card)
    {
        //Add another copy of this card to hand   — On reveal
        if (card.PlacedOpponentArea != null)
        {
            GameObject cardPrefab = CharacterManager.Instance.GetCharacterPrefab(card.CharacterName);
            OpponentManager.Instance.SpawnCard(cardPrefab);
            Debug.Log($"{card.name} has been duplicated and added to the opponent's hand.");
        }
        else if (card.PlacedOpponentArea == null && card.PlacedArea != null)
        {
            Card cardCopy = Object.Instantiate(card);
            DeckManager.Instance.SpawnCard(cardCopy);
            Debug.Log($"{card.name} has been duplicated and added to the hand.");
        }
    }
}
