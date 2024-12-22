using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDeckData", menuName = "DeckManager/PlayerDeck")]
public class PlayerDeckData : ScriptableObject
{
    public List<GameObject> PlayerDeck = new List<GameObject>();
}
