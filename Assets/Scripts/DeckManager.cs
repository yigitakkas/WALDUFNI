using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [Header("Card Prefabs")]
    public List<GameObject> CardPrefabs;

    [Header("Spawn Points")]
    public List<Transform> SpawnPoints;

    private List<GameObject> _deck = new List<GameObject>();

    void Start()
    {
        CreateDeck();
        SpawnRandomCards();
    }

    private void CreateDeck()
    {
        foreach (GameObject cardPrefab in CardPrefabs)
        {
            _deck.Add(cardPrefab);
        }
    }

    private void SpawnRandomCards()
    {
        if (SpawnPoints.Count < 3)
        {
            Debug.LogError("Yeterli spawn noktasý yok! En az 3 tane gerekli.");
            return;
        }

        for (int i = 0; i < 3; i++)
        {
            if (_deck.Count == 0)
            {
                Debug.LogError("Deste bitti! Daha fazla kart yok.");
                break;
            }

            int randomIndex = Random.Range(0, _deck.Count);
            GameObject randomCard = _deck[randomIndex];

            Instantiate(randomCard, SpawnPoints[i].position, Quaternion.identity);

            _deck.RemoveAt(randomIndex);
        }
    }
}
