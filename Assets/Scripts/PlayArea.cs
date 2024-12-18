using UnityEngine;
using System.Collections.Generic;

public class PlayArea : MonoBehaviour
{
    [SerializeField]
    private List<SnapPoint> _snapPoints = new List<SnapPoint>();
    private List<Card> _placedCards = new List<Card>();
    private List<Card> _placedCardsThisRound = new List<Card>();
    public List<Card> PlacedCards
    {
        get => _placedCards;
    }
    public List<Card> PlacedCardsThisRound
    {
        get => _placedCardsThisRound;
    }
    public int Index;
    [SerializeField]
    private bool _playedHereThisRound=false;
    public bool PlayedHereThisRound
    {
        get => _playedHereThisRound;
        private set => _playedHereThisRound = value;
    }
    [SerializeField]
    private int _playedAmountThisRound = 0;
    private void OnEnable()
    {
        RoundManager.OnRoundEnded += ResetPlayedHere;
    }

    private void OnDisable()
    {
        RoundManager.OnRoundEnded -= ResetPlayedHere;
    }
    private void ResetPlayedHere()
    {
        _placedCardsThisRound.Clear();
        _playedHereThisRound = false;
        _playedAmountThisRound = 0;
    }
    private void Start()
    {
        FillSnapPoints();
    }
    public bool IsPointInPlayArea(Vector3 point)
    {
        Collider2D collider = GetComponent<Collider2D>();
        return collider != null && collider.bounds.Contains(point);
    }

    public Vector3 GetSnapPosition(Card card)
    {
        Vector3 position=Vector3.zero;
        if (_snapPoints != null && _snapPoints.Count > 0)
        {
            foreach (SnapPoint snapPoint in _snapPoints)
            {
                if (!snapPoint.Occupied)
                {
                    position = snapPoint.Transform.position;
                    snapPoint.Occupied = true;
                    snapPoint.AssignedCard = card;
                    break;
                }
            }
        }
        return position;
    }

    private void FillSnapPoints()
    {
        Transform snapPoint1 = transform.Find("SnapPoint1");
        Transform snapPoint2 = transform.Find("SnapPoint2");
        Transform snapPoint3 = transform.Find("SnapPoint3");

        if (snapPoint1 != null) _snapPoints.Add(new SnapPoint(snapPoint1));
        if (snapPoint2 != null) _snapPoints.Add(new SnapPoint(snapPoint2));
        if (snapPoint3 != null) _snapPoints.Add(new SnapPoint(snapPoint3));
    }

    public void PlaceCard(Card card)
    {
        _placedCards.Add(card);
        _placedCardsThisRound.Add(card);
        _playedHereThisRound = true;
        _playedAmountThisRound++;
    }

    public void RemoveCard(Card card)
    {
        _placedCards.Remove(card);
        _placedCardsThisRound.Remove(card);
        _playedAmountThisRound--;
        if(_playedAmountThisRound==0)
        {
            _playedHereThisRound = false;
        }
        foreach(SnapPoint snapPoint in _snapPoints)
        {
            if(snapPoint.AssignedCard == card)
            {
                snapPoint.AssignedCard = null;
                snapPoint.Occupied = false;
            }
        }
        CheckEmptySnapPoints();
    }

    private void CheckEmptySnapPoints()
    {
        for (int i = 0; i < _snapPoints.Count - 1; i++)
        {
            SnapPoint currentSnapPoint = _snapPoints[i];
            SnapPoint nextSnapPoint = _snapPoints[i + 1];

            if (!currentSnapPoint.Occupied && nextSnapPoint.Occupied)
            {
                MoveCardToSnapPoint(nextSnapPoint.AssignedCard, currentSnapPoint);

                nextSnapPoint.AssignedCard = null;
                nextSnapPoint.Occupied = false;
            }
        }
    }

    private void MoveCardToSnapPoint(Card card, SnapPoint targetSnapPoint)
    {
        if (card != null && targetSnapPoint != null)
        {
            card.transform.position = targetSnapPoint.Transform.position;
            targetSnapPoint.AssignedCard = card;
            targetSnapPoint.Occupied = true;
        }
    }

    public Vector3 GetCardPosition(Card card)
    {
        foreach (SnapPoint snapPoint in _snapPoints)
        {
            if(snapPoint.AssignedCard == card)
            {
                return snapPoint.Transform.position;
            }
        }
        return Vector3.zero;
    }

    public int PlacedAmount()
    {
        return _placedCards.Count;
    }

    public void PlaceCardsIntoList(List<Card> cards)
    {
        foreach(Card card in _placedCards)
        {
            cards.Add(card);
        }
    }

    public int PlacedCardsPower()
    {
        int sum = 0;
        foreach (Card card in _placedCards)
        {
            sum += card.GetComponent<CardDisplay>().Power;
        }
        return sum;
    }

    public bool CheckSnapPointsAvailability()
    {
        int occupiedAmount = 0;
        foreach(SnapPoint snapPoint in _snapPoints)
        {
            if (snapPoint.Occupied)
                occupiedAmount++;
        }
        if (occupiedAmount == 3)
            return false;
        else
            return true;
    }


    [System.Serializable]
    private class SnapPoint
    {
        public Transform Transform;
        public bool Occupied;
        public Card AssignedCard;
        public SnapPoint(Transform transform)
        {
            this.Transform = transform;
            this.Occupied = false;
        }
    }
}
