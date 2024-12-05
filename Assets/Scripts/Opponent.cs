using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opponent : MonoBehaviour
{
    public static Opponent Instance;
    private List<GameObject> _opponentDeck = new List<GameObject>();
    private PlayArea _opponentFirstArea;
    private PlayArea _opponentSecondArea;
    private PlayArea _opponentThirdArea;
    private List<PlayArea> _opponentAreas = new List<PlayArea>();

    private void Awake()
    {
        Instance = this;
        FindAreas();
    }

    private void FindAreas()
    {
        _opponentFirstArea = transform.Find("OpponentArea1").GetComponent<PlayArea>();
        _opponentSecondArea = transform.Find("OpponentArea2").GetComponent<PlayArea>();
        _opponentThirdArea = transform.Find("OpponentArea3").GetComponent<PlayArea>();
        _opponentAreas.Add(_opponentFirstArea);
        _opponentAreas.Add(_opponentSecondArea);
        _opponentAreas.Add(_opponentThirdArea);
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
            GameObject newCardObject = Instantiate(_opponentDeck[randomIndex], GetRandomTargetPosition(), Quaternion.identity);
            newCardObject.GetComponent<Card>()?.DestroyCollider();
            _opponentDeck.RemoveAt(randomIndex);
        }
    }

    private Vector3 GetRandomTargetPosition()
    {
        int randomIndex = Random.Range(0, _opponentAreas.Count);
        PlayArea randomArea = _opponentAreas[randomIndex];
        Card placeholderCard = null;  // Random kart yerine sahte bir kart kullanýlýyor
        return CheckSnapPoints(randomArea, placeholderCard);
    }


    private Vector3 CheckSnapPoints(PlayArea area, Card card)
    {
        return area.GetSnapPosition(card);
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
