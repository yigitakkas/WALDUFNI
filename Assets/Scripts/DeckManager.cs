using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [Header("Card Prefabs")]
    public List<GameObject> CardPrefabs;

    [Header("Spawn Points")]
    public List<Transform> SpawnPoints;

    [SerializeField]
    private List<GameObject> _playerDeck = new List<GameObject>();
    [SerializeField]
    private List<GameObject> _basicCards = new List<GameObject>();
    [SerializeField]
    private List<GameObject> _powerCards = new List<GameObject>();
    [SerializeField]
    private List<GameObject> _specialCards = new List<GameObject>();

    private void OnEnable()
    {
        RoundManager.OnRoundEnded += SpawnRandomCards;
    }

    private void OnDisable()
    {
        RoundManager.OnRoundEnded -= SpawnRandomCards;
    }
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
            DefineCardClasses(cardPrefab);
        }
        Opponent.Instance.AssignDeck(new List<GameObject>(_playerDeck));
    }

    private void DefineCardClasses(GameObject cardPrefab)
    {
        Card.CardClass cardClass = cardPrefab.GetComponent<Card>().CardClassType;
        if (cardClass == Card.CardClass.Basic)
            _basicCards.Add(cardPrefab);
        else if (cardClass == Card.CardClass.Power)
            _powerCards.Add(cardPrefab);
        else if (cardClass == Card.CardClass.Special)
            _specialCards.Add(cardPrefab);
    }

    private void SpawnRandomCards()
    {
        if (SpawnPoints.Count < 3)
        {
            Debug.LogError("Yeterli spawn noktas� yok! En az 3 tane gerekli.");
            return;
        }
        if (_playerDeck.Count == 0)
        {
            Debug.LogError("Deste bitti! Daha fazla kart yok.");
            return;
        }

        int currentRound = RoundManager.Instance.CurrentRound;
        Debug.Log(currentRound);
        switch (currentRound)
        {
            case 1:
                RoundOne();
                break;
            case 2:
                SpawnSingleCard(60, 35);
                break;
            case 3:
                SpawnSingleCard(30, 45);
                break;
            case 4:
                SpawnSingleCard(5, 35);
                break;
            default:
                Debug.LogWarning("Ge�ersiz round numaras�!");
                break;
        }
    }

    private void RoundOne()
    {
        int spawnIndex = 0;

        float randomValue = Random.value * 100;
        if (randomValue < 30) // %30: 3 Basic Cards
        {
            for (int i = 0; i < 3; i++) 
                AddCardToSpawn(_basicCards, ref spawnIndex);
        }
        else if (randomValue < 70) // %40: 2 Basic, 1 Power
        {
            for (int i = 0; i < 2; i++) 
                AddCardToSpawn(_basicCards, ref spawnIndex);
            AddCardToSpawn(_powerCards, ref spawnIndex);
        }
        else if (randomValue < 90) // %20: 1 Basic, 2 Power
        {
            AddCardToSpawn(_basicCards, ref spawnIndex);
            for (int i = 0; i < 2; i++) 
                AddCardToSpawn(_powerCards, ref spawnIndex);
        }
        else if (randomValue < 99) // %9: 1 Basic, 1 Power, 1 Special
        {
            AddCardToSpawn(_basicCards, ref spawnIndex);
            AddCardToSpawn(_powerCards, ref spawnIndex);
            AddCardToSpawn(_specialCards, ref spawnIndex);
        }
        else // %1: 1 Basic, 2 Special
        {
            AddCardToSpawn(_basicCards, ref spawnIndex);
            for (int i = 0; i < 2; i++) 
                AddCardToSpawn(_specialCards, ref spawnIndex);
        }
    }

    private void SpawnSingleCard(int basicChance, int powerChance)
    {
        float randomValue = Random.value * 100;
        int spawnIndex = 0; 

        if (randomValue < basicChance && _basicCards.Count > 0)
        {
            AddCardToSpawn(_basicCards, ref spawnIndex);
        }
        else if (randomValue < basicChance + powerChance && _powerCards.Count > 0)
        {
            AddCardToSpawn(_powerCards, ref spawnIndex);
        }
        else if (_specialCards.Count > 0)
        {
            AddCardToSpawn(_specialCards, ref spawnIndex);
        }
    }

    private void AddCardToSpawn(List<GameObject> cardList, ref int spawnIndex)
    {
        if (cardList.Count == 0 || spawnIndex >= SpawnPoints.Count) 
            return;

        int randomIndex = Random.Range(0, cardList.Count);
        GameObject randomCard = cardList[randomIndex];

        Instantiate(randomCard, SpawnPoints[spawnIndex].position, Quaternion.identity);
        cardList.RemoveAt(randomIndex);

        spawnIndex++;
    }

}
