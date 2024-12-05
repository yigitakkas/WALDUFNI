using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opponent : MonoBehaviour
{
    public static Opponent Instance;

    private List<GameObject> _opponentDeck = new List<GameObject>();
    private List<PlayArea> _opponentAreas = new List<PlayArea>();
    private PlayArea _placedArea;

    [SerializeField]
    private List<GameObject> _opponentHand = new List<GameObject>();
    [SerializeField]
    private List<GameObject> _basicCards = new List<GameObject>();
    [SerializeField]
    private List<GameObject> _powerCards = new List<GameObject>();
    [SerializeField]
    private List<GameObject> _specialCards = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
        FindAreas();
    }
    void Start()
    {
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
        _opponentAreas.Add(transform.Find("OpponentArea1").GetComponent<PlayArea>());
        _opponentAreas.Add(transform.Find("OpponentArea2").GetComponent<PlayArea>());
        _opponentAreas.Add(transform.Find("OpponentArea3").GetComponent<PlayArea>());
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
        Card.CardClass cardClass = card.GetComponent<Card>().CardClassType;
        if (cardClass == Card.CardClass.Basic)
            _basicCards.Add(card);
        else if (cardClass == Card.CardClass.Power)
            _powerCards.Add(card);
        else if (cardClass == Card.CardClass.Special)
            _specialCards.Add(card);
    }
    public void DrawOpponentCards()
    {
        int currentRound = RoundManager.Instance.CurrentRound;

        if (currentRound == 1)
        {
            float randomValue = Random.value * 100;
            if (randomValue < 30) // %30: 3 Basic Cards
            {
                for (int i = 0; i < 3; i++) AddCardToOpponentHand(_basicCards);
            }
            else if (randomValue < 70) // %40: 2 Basic, 1 Power
            {
                for (int i = 0; i < 2; i++) AddCardToOpponentHand(_basicCards);
                AddCardToOpponentHand(_powerCards);
            }
            else if (randomValue < 90) // %20: 1 Basic, 2 Power
            {
                AddCardToOpponentHand(_basicCards);
                for (int i = 0; i < 2; i++) AddCardToOpponentHand(_powerCards);
            }
            else if (randomValue < 99) // %9: 1 Basic, 1 Power, 1 Special
            {
                AddCardToOpponentHand(_basicCards);
                AddCardToOpponentHand(_powerCards);
                AddCardToOpponentHand(_specialCards);
            }
            else // %1: 1 Basic, 2 Special
            {
                AddCardToOpponentHand(_basicCards);
                for (int i = 0; i < 2; i++) AddCardToOpponentHand(_specialCards);
            }
        }
        else
        {
            float randomValue = Random.value * 100;

            if (currentRound == 2) // Round 2: %60 Basic, %35 Power, %5 Special
            {
                AddSingleCardBasedOnChance(randomValue, 60, 35, 5);
            }
            else if (currentRound == 3) // Round 3: %30 Basic, %45 Power, %25 Special
            {
                AddSingleCardBasedOnChance(randomValue, 30, 45, 25);
            }
            else if (currentRound == 4) // Round 4: %5 Basic, %35 Power, %60 Special
            {
                AddSingleCardBasedOnChance(randomValue, 5, 35, 60);
            }
        }
    }
    private void AddCardToOpponentHand(List<GameObject> cardList)
    {
        if (cardList.Count == 0) return;

        int randomIndex = Random.Range(0, cardList.Count);
        GameObject randomCard = cardList[randomIndex];
        _opponentHand.Add(randomCard);
        cardList.RemoveAt(randomIndex);
    }

    private void AddSingleCardBasedOnChance(float randomValue, int basicChance, int powerChance, int specialChance)
    {
        if (randomValue < basicChance && _basicCards.Count > 0)
        {
            AddCardToOpponentHand(_basicCards);
        }
        else if (randomValue < basicChance + powerChance && _powerCards.Count > 0)
        {
            AddCardToOpponentHand(_powerCards);
        }
        else if (_specialCards.Count > 0)
        {
            AddCardToOpponentHand(_specialCards);
        }
    }
    public void PlayOpponentCard()
    {
        if (_opponentHand.Count > 0)
        {
            GameObject cardToPlay = _opponentHand[Random.Range(0, _opponentHand.Count)];
            _opponentHand.Remove(cardToPlay);

            PlayCardToOpponentArea(cardToPlay);
        }
    }
    private void PlayCardToOpponentArea(GameObject card)
    {
        GameObject newCardObject = Instantiate(card, transform.position, Quaternion.identity);
        Card newCard = newCardObject.GetComponent<Card>();

        newCardObject.transform.position = GetRandomTargetPosition(newCard);
        _placedArea.PlaceCard(newCard);
        newCard?.DestroyCollider(_placedArea);
    }



    private Vector3 GetRandomTargetPosition(Card card)
    {
        List<PlayArea> availableOpponentAreas = new List<PlayArea>();
        foreach (PlayArea area in _opponentAreas)
        {
            if(area.CheckSnapPointsAvailability())
            {
                availableOpponentAreas.Add(area);
            }
        }
        if (availableOpponentAreas.Count != 0)
        {
            int randomIndex = Random.Range(0, availableOpponentAreas.Count);
            PlayArea randomArea = availableOpponentAreas[randomIndex];
            _placedArea = randomArea;
            return CheckSnapPoints(randomArea, card);
        }
        else
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
