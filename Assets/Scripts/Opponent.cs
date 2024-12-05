using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opponent : MonoBehaviour
{
    public static Opponent Instance;

    private List<GameObject> _opponentDeck = new List<GameObject>();
    private List<PlayArea> _opponentAreas = new List<PlayArea>();
    private PlayArea _placedArea;

    private void Awake()
    {
        Instance = this;
        FindAreas();
    }
    private void OnEnable()
    {
        RoundManager.OnRoundStarted += PlayHand;
    }

    private void OnDisable()
    {
        RoundManager.OnRoundStarted -= PlayHand;
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
    }

    public void PlayHand()
    {
        for (int i = 0; i < 3 && _opponentDeck.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, _opponentDeck.Count);
            GameObject newCardObject = Instantiate(_opponentDeck[randomIndex], transform.position, Quaternion.identity);
            Card newCard = newCardObject.GetComponent<Card>();
            newCardObject.transform.position = GetRandomTargetPosition(newCard);
            _placedArea.PlaceCard(newCard);
            newCard?.DestroyCollider(_placedArea);
            _opponentDeck.RemoveAt(randomIndex);
        }
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
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
