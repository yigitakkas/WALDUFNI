using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance;
    [Header("Card Prefabs")]
    public List<GameObject> CardPrefabs;
    public Transform SpawnPosition;

    [SerializeField]
    private List<GameObject> _playerDeck = new List<GameObject>();
    [SerializeField]
    private List<GameObject> _basicCards = new List<GameObject>();
    [SerializeField]
    private List<GameObject> _powerCards = new List<GameObject>();
    [SerializeField]
    private List<GameObject> _specialCards = new List<GameObject>();

    private List<GameObject> _spawnedCards = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
    }
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
        OpponentManager.Instance.AssignDeck(new List<GameObject>(_playerDeck));
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
        if (!ValidateDeckCount())
            return;

        RemovePlayedCards();

        int currentRound = RoundManager.Instance.CurrentRound;

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
                Debug.LogWarning("Geçersiz round numarasý!");
                break;
        }

        ArrangeCards(); // Spawnlanan kartlarý sýralama
    }

    private void RemovePlayedCards()
    {
        List<GameObject> cardsToRemove = new List<GameObject>(); // Kaldýrýlacak kartlarý geçici olarak tutar

        foreach (GameObject card in _spawnedCards)
        {
            if (card.GetComponent<Card>().PlacedArea != null)
            {
                cardsToRemove.Add(card); // Kaldýrýlacak kartlarý listeye ekle
            }
        }

        // Döngü tamamlandýktan sonra listedeki kartlarý asýl listeden kaldýr
        foreach (GameObject card in cardsToRemove)
        {
            _spawnedCards.Remove(card);
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
        if (cardList.Count == 0)
            return;

        int randomIndex = Random.Range(0, cardList.Count);
        GameObject randomCard = cardList[randomIndex];
        GameObject spawnedCard = Instantiate(randomCard, SpawnPosition.position, Quaternion.identity);
        spawnedCard.transform.SetParent(transform);
        _spawnedCards.Add(spawnedCard); // Spawnlanan kartý listeye ekle
        cardList.RemoveAt(randomIndex);

        spawnIndex++;
    }

    private void ArrangeCards()
    {
        int cardCount = _spawnedCards.Count;
        if (cardCount == 0)
            return;

        float spacing = 2.0f; // Kartlar arasýndaki temel mesafe
        float centerOffset = (cardCount - 1) * spacing / 2.0f;

        for (int i = 0; i < cardCount; i++)
        {
            Vector3 targetPosition = new Vector3(transform.position.x + (i * spacing) - centerOffset, transform.position.y, transform.position.z);
            _spawnedCards[i].transform.DOMove(targetPosition, 0.5f); // DOTween ile hareket
        }
    }

    private bool ValidateDeckCount()
    {
        if (_playerDeck.Count == 0)
        {
            Debug.LogError("Deste bitti! Daha fazla kart yok.");
            return false;
        }
        return true;
    }
}
