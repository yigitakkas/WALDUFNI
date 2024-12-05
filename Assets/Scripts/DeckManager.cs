using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [Header("Card Prefabs")]
    public List<GameObject> CardPrefabs;

    [Header("Spawn Points")]
    public List<Transform> SpawnPoints;

    private List<GameObject> _playerDeck = new List<GameObject>();

    void Start()
    {
        CreateDeck();
        SpawnRandomCards();
    }

    private void CreateDeck()
    {
        foreach (GameObject cardPrefab in CardPrefabs)
        {
            _playerDeck.Add(cardPrefab);
        }
        Opponent.Instance.AssignDeck(_playerDeck);
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
            if (_playerDeck.Count == 0)
            {
                Debug.LogError("Deste bitti! Daha fazla kart yok.");
                break;
            }

            int randomIndex = Random.Range(0, _playerDeck.Count);
            GameObject randomCard = _playerDeck[randomIndex];

            Instantiate(randomCard, SpawnPoints[i].position, Quaternion.identity);

            _playerDeck.RemoveAt(randomIndex);
        }
    }
}
