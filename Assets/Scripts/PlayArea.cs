using UnityEngine;
using System.Collections.Generic;

public class PlayArea : MonoBehaviour
{
    [SerializeField]
    private List<SnapPoint> _snapPoints = new List<SnapPoint>();
    private List<Card> _placedCards = new List<Card>();

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
    }

    public void RemoveCard(Card card)
    {
        _placedCards.Remove(card);
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
