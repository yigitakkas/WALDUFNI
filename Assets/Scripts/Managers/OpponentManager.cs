using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UnityEditor;

public class OpponentManager : MonoBehaviour
{
    public static OpponentManager Instance;

    private List<GameObject> _opponentDeck = new List<GameObject>();
    private List<PlayArea> _opponentAreas = new List<PlayArea>();
    private PlayArea _placedArea;
    public Transform SpawnPosition;
    private GameObject _opponentCardParent;
    public GameObject OpponentCardParent
    {
        get => _opponentCardParent;
    }

    [SerializeField]
    private List<GameObject> _opponentHand = new List<GameObject>();
    [SerializeField]
    private List<GameObject> _basicCards = new List<GameObject>();
    [SerializeField]
    private List<GameObject> _powerCards = new List<GameObject>();
    [SerializeField]
    private List<GameObject> _specialCards = new List<GameObject>();

    public GameObject EchoCard;
    private GameObject _pioneerCard;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        FindAreas();
        CreateOpponentParent();
    }

    private void CreateOpponentParent()
    {
        if (_opponentCardParent == null)
        {
            _opponentCardParent = new GameObject("OpponentCardsContainer");
        }
    }

    void Start()
    {
        CheckPioneer();
        DrawOpponentCards();
    }

    private void OnEnable()
    {
        RoundManager.OnRoundEnded += DrawOpponentCards;
        RoundManager.OnRoundStarted += PlayOpponentCard;
    }

    private void OnDisable()
    {
        RoundManager.OnRoundEnded -= DrawOpponentCards;
        RoundManager.OnRoundStarted -= PlayOpponentCard;
    }

    private void FindAreas()
    {
        PlayArea[] areas = GetComponentsInChildren<PlayArea>();
        foreach (PlayArea area in areas)
        {
            if (area.CompareTag("OpponentArea"))
                _opponentAreas.Add(area);
        }
    }

    public void AssignDeck(List<GameObject> deck)
    {
        _opponentDeck = deck;
        foreach (GameObject card in _opponentDeck)
        {
            DefineCardClasses(card);
        }
    }

    private void DefineCardClasses(GameObject card)
    {
        CardClass cardClass = card.GetComponent<Card>().CardClassType;
        if (cardClass == CardClass.Basic)
            _basicCards.Add(card);
        else if (cardClass == CardClass.Power)
            _powerCards.Add(card);
        else if (cardClass == CardClass.Special)
            _specialCards.Add(card);
    }

    public void DrawOpponentCards()
    {

        int currentRound = RoundManager.Instance.CurrentRound;

        if (currentRound == 1)
        {
            float randomValue = Random.value * 100;
            if (randomValue < 30) // %30: 2 Basic Cards
            {
                for (int i = 0; i < 2; i++) 
                    AddCardToOpponentHand(_basicCards);
            }
            else if (randomValue < 70) // %40: 1 Basic, 1 Power
            {
                AddCardToOpponentHand(_basicCards);
                AddCardToOpponentHand(_powerCards);
            }
            else if (randomValue < 90) // %20: 1 Basic, 1 Special
            {
                AddCardToOpponentHand(_basicCards);
                AddCardToOpponentHand(_specialCards);
            }
            else if (randomValue < 99) // %9: 2 Power
            {
                for (int i = 0; i < 2; i++)
                    AddCardToOpponentHand(_powerCards);
            }
            else // %1: 1 Power, 1 Special
            {
                AddCardToOpponentHand(_powerCards);
                AddCardToOpponentHand(_specialCards);
            }
        }
        else
        {
            float randomValue = Random.value * 100;

            if (currentRound == 2) // Round 2: %60 Basic, %35 Power, %5 Special
            {
                AddSingleCardBasedOnChance(randomValue, 60, 35, 5);
            }
            else if (currentRound == 3) // Round 3: %25 Basic, %45 Power, %25 Special
            {
                AddSingleCardBasedOnChance(randomValue, 30, 50, 20);
            }
            else if (currentRound == 4) // Round 4: %5 Basic, %35 Power, %60 Special
            {
                AddSingleCardBasedOnChance(randomValue, 15, 35, 50);
            }
            else if (currentRound == 5)
            {
                AddSingleCardBasedOnChance(randomValue, 5, 10, 85);
            }
        }
    }

    private void CheckPioneer()
    {
        foreach (GameObject gameObject in _opponentDeck)
        {
            CardEffect cardEffect = gameObject.GetComponent<Card>().CardEffectType;
            if (cardEffect == CardEffect.Pioneer)
                _pioneerCard = gameObject;
        }
        if (_pioneerCard == null) return;
        _opponentHand.Add(_pioneerCard);
        if (_basicCards.Contains(_pioneerCard))
            _basicCards.Remove(_pioneerCard);
        else if (_powerCards.Contains(_pioneerCard))
            _powerCards.Remove(_pioneerCard);
        else if (_specialCards.Contains(_pioneerCard))
            _specialCards.Remove(_pioneerCard);
    }

    private void AddCardToOpponentHand(List<GameObject> cardList)
    {
        if (cardList.Count == 0) return;

        int randomIndex = Random.Range(0, cardList.Count);
        GameObject randomCard = cardList[randomIndex];
        _opponentHand.Add(randomCard);
        cardList.RemoveAt(randomIndex);
    }

    public void SpawnCard(GameObject card)
    {
        _opponentHand.Add(card);
    }

    private void AddSingleCardBasedOnChance(float randomValue, int basicChance, int powerChance, int specialChance)
    {
        bool cardAdded = false;

        if (randomValue < basicChance && _basicCards.Count > 0)
        {
            AddCardToOpponentHand(_basicCards);
            cardAdded = true;
        }
        else if (randomValue < basicChance + powerChance && _powerCards.Count > 0)
        {
            AddCardToOpponentHand(_powerCards);
            cardAdded = true;
        }
        else if (_specialCards.Count > 0)
        {
            AddCardToOpponentHand(_specialCards);
            cardAdded = true;
        }

        if(!cardAdded)
        {
            AddFallbackCard();
        }
    }

    private void AddFallbackCard()
    {
        if (_basicCards.Count > 0)
        {
            AddCardToOpponentHand(_basicCards);
        }
        else if (_powerCards.Count > 0)
        {
            AddCardToOpponentHand(_powerCards);
        }
        else if (_specialCards.Count > 0)
        {
            AddCardToOpponentHand(_specialCards);
        }
        else
        {
            Debug.LogWarning("Tüm destelerde kart kalmadý!");
        }
    }

    public void PlayOpponentCard()
    {
        while (true)
        {
            List<GameObject> playableCards = _opponentHand
                .Where(card => card.GetComponent<Card>().Energy <= EnergyManager.Instance.OpponentEnergy)
                .ToList();

            if (playableCards.Count == 0)
            {
                Debug.LogWarning("OpponentManager'ýn enerjisi yeten kartý yok!");
                break;
            }

            GameObject cardToPlay = playableCards
            .GroupBy(card => card.GetComponent<Card>().Power)  // Power deðerine göre gruplama
            .OrderByDescending(group => group.Key)             // En yüksek Power grubunu seç
            .First()                                           // Ýlk (en yüksek Power) grubu al
            .OrderBy(_ => Random.value)                        // Grup içinden rastgele bir kart seç
            .First();


            _opponentHand.Remove(cardToPlay);

            int cardEnergy = cardToPlay.GetComponent<Card>().Energy;
            EnergyManager.Instance.DecreaseEnergy(cardEnergy, player: false);

            Debug.Log("OpponentManager oynadý: " + cardToPlay.name);

            PlayCardToOpponentArea(cardToPlay);

            if (EnergyManager.Instance.OpponentEnergy <= 0)
            {
                break;
            }
        }
    }



    private void PlayCardToOpponentArea(GameObject card)
    {
        GameObject newCardObject = Instantiate(card, SpawnPosition.position, Quaternion.identity, _opponentCardParent.transform);
        Card newCard = newCardObject.GetComponent<Card>();

        Vector3 targetPosition = GetRandomTargetPosition(newCard);
        if (targetPosition == Vector3.zero) return;
        newCardObject.transform.DOMove(targetPosition, 0.5f);

        _placedArea.PlaceCard(newCard);
        newCard.Played = true;
        newCard.SetOpponentArea(_placedArea);
    }

    private Vector3 GetRandomTargetPosition(Card card)
    {
        List<PlayArea> availableOpponentAreas = _opponentAreas
            .Where(area => area.CheckSnapPointsAvailability())
            .ToList();

        List<PlayArea> activeOpponentAreas = availableOpponentAreas
        .Where(area => area._correspondingBattleground.BattlegroundEffect != BattlegroundEffect.None)
        .ToList();

        PlayArea beastLairsArea = availableOpponentAreas
            .FirstOrDefault(area =>
                area._correspondingBattleground.BattlegroundEffect == BattlegroundEffect.BeastLair &&
                area.Index == RoundManager.Instance.CurrentRound);

        List<PlayArea> targetAreas = activeOpponentAreas.Count > 0 ? activeOpponentAreas : availableOpponentAreas;

        if (targetAreas.Count > 0)
        {
            PlayArea selectedArea = beastLairsArea ?? targetAreas[Random.Range(0, targetAreas.Count)];
            _placedArea = selectedArea;

            return CheckSnapPoints(selectedArea, card);
        }

        Debug.LogWarning("Hiçbir opponent alaný uygun deðil!");
        return Vector3.zero;
    }


    private Vector3 CheckSnapPoints(PlayArea area, Card card)
    {
        return area.GetSnapPosition(card);
    }

    public List<PlayArea> ReturnOpponentAreas()
    {
        return _opponentAreas;
    }
}
