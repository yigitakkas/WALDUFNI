using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance;
    [Header("Card Prefabs")]
    public List<GameObject> CardPrefabs;
    public Transform SpawnPosition;

    private GameObject _playerCardParent;
    public GameObject PlayerCardParent
    {
        get => _playerCardParent;
    }

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
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        CreatePlayerParent();
    }

    private void CreatePlayerParent()
    {
        if (_playerCardParent == null)
        {
            _playerCardParent = new GameObject("PlayerCardsContainer");
        }
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
        CardClass cardClass = cardPrefab.GetComponent<Card>().CardClassType;
        if (cardClass == CardClass.Basic)
            _basicCards.Add(cardPrefab);
        else if (cardClass == CardClass.Power)
            _powerCards.Add(cardPrefab);
        else if (cardClass == CardClass.Special)
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
        SetDarknessOfCards(EnergyManager.Instance.PlayerEnergy);
    }

    private void RemovePlayedCards()
    {
        List<GameObject> cardsToRemove = new List<GameObject>(); 

        foreach (GameObject card in _spawnedCards)
        {
            if (card.GetComponent<Card>().PlacedArea != null)
            {
                cardsToRemove.Add(card);
            }
        }

        foreach (GameObject card in cardsToRemove)
        {
            //card.GetComponent<Card>().Played = true;
            _spawnedCards.Remove(card);
        }
    }

    private void RoundOne()
    {
        float randomValue = Random.value * 100;
        if (randomValue < 30) // %30: 3 Basic Cards
        {
            for (int i = 0; i < 3; i++)
                AddCardToSpawn(_basicCards);
        }
        else if (randomValue < 70) // %40: 2 Basic, 1 Power
        {
            for (int i = 0; i < 2; i++)
                AddCardToSpawn(_basicCards);
            AddCardToSpawn(_powerCards);
        }
        else if (randomValue < 90) // %20: 1 Basic, 2 Power
        {
            AddCardToSpawn(_basicCards);
            for (int i = 0; i < 2; i++)
                AddCardToSpawn(_powerCards);
        }
        else if (randomValue < 99) // %9: 1 Basic, 1 Power, 1 Special
        {
            AddCardToSpawn(_basicCards);
            AddCardToSpawn(_powerCards);
            AddCardToSpawn(_specialCards);
        }
        else // %1: 1 Basic, 2 Special
        {
            AddCardToSpawn(_basicCards);
            for (int i = 0; i < 2; i++)
                AddCardToSpawn(_specialCards);
        }
    }

    private void SpawnSingleCard(int basicChance, int powerChance)
    {
        float randomValue = Random.value * 100;
        bool cardAdded = false;

        if (randomValue < basicChance && _basicCards.Count > 0)
        {
            AddCardToSpawn(_basicCards);
            cardAdded = true;
        }
        else if (randomValue < basicChance + powerChance && _powerCards.Count > 0)
        {
            AddCardToSpawn(_powerCards);
            cardAdded = true;
        }
        else if (_specialCards.Count > 0)
        {
            AddCardToSpawn(_specialCards);
            cardAdded = true;
        }

        if(!cardAdded)
        {
            AddFallbackCard();
        }
    }

    public void SpawnCard(Card card)
    {
        card.transform.position = SpawnPosition.position;
        card.transform.SetParent(_playerCardParent.transform);
        _spawnedCards.Add(card.gameObject);
        //ArrangeCards();
    }
    private void AddFallbackCard()
    {
        if (_basicCards.Count > 0)
        {
            AddCardToSpawn(_basicCards);
        }
        else if (_powerCards.Count > 0)
        {
            AddCardToSpawn(_powerCards);
        }
        else if (_specialCards.Count > 0)
        {
            AddCardToSpawn(_specialCards);
        }
        else
        {
            Debug.LogWarning("Tüm destelerde kart kalmadý!");
        }
    }

    private void AddCardToSpawn(List<GameObject> cardList)
    {
        if (cardList.Count == 0)
            return;

        int randomIndex = Random.Range(0, cardList.Count);
        GameObject randomCard = cardList[randomIndex];
        GameObject spawnedCard = Instantiate(randomCard, SpawnPosition.position, Quaternion.identity, _playerCardParent.transform);
        _spawnedCards.Add(spawnedCard);
        cardList.RemoveAt(randomIndex);
    }

    private void ArrangeCards()
    {
        int cardCount = _spawnedCards.Count;
        if (cardCount == 0)
            return;

        float spacing = 2.0f;
        float centerOffset = (cardCount - 1) * spacing / 2.0f;

        for (int i = 0; i < cardCount; i++)
        {
            Vector3 targetPosition = new Vector3(transform.position.x + (i * spacing) - centerOffset, transform.position.y, transform.position.z);
            _spawnedCards[i].GetComponent<Card>().SetOriginalPosition(targetPosition);
            _spawnedCards[i].transform.DOMove(targetPosition, 0.5f);
        }
    }

    public void SetDarknessOfCards(int playerEnergy)
    {
        int cardCount = _spawnedCards.Count;
        if (cardCount == 0)
            return;
        for (int i = 0; i < cardCount; i++)
        {
            CardDisplay cardDisplay = _spawnedCards[i].GetComponent<CardDisplay>();
            if (cardDisplay.Energy <= playerEnergy)
            {
                cardDisplay.LightenObject();
            } else
            {
                cardDisplay.DarkenObject();
            }
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
