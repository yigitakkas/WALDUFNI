using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class ProxyEffect : ICardEffect
{
    private List<PlayArea> _playAreas = new List<PlayArea>();
    int _index = 0;
    GameObject _phantomCard;
    Transform _spawnPoint;
    public void ApplyEffect(Card card)
    {
        //Sends 1-power 2 cards to other areas — On reveal
        _phantomCard = DeckManager.Instance.PhantomCard;

        _playAreas.Clear();

        if (card.PlacedOpponentArea != null)
        {
            _playAreas = new List<PlayArea>(RoundManager.Instance.OpponentPlayAreas);
            _index = card.PlacedOpponentArea.Index;

            List<int> possibleIndices = new List<int> { 1, 2, 3 };
            possibleIndices.Remove(_index);

            System.Random random = new System.Random();
            possibleIndices = possibleIndices.OrderBy(x => random.Next()).ToList();

            PlayArea selectedPlayArea = null;
            foreach (int index in possibleIndices)
            {
                PlayArea playArea = _playAreas[index - 1];
                if (playArea.CheckSnapPointsAvailability())
                {
                    selectedPlayArea = playArea;
                    break;
                }
            }
            if (selectedPlayArea != null)
            {
                _spawnPoint = OpponentManager.Instance.SpawnPosition;
                GameObject cardObject = GameObject.Instantiate(_phantomCard, _spawnPoint.position, Quaternion.identity, OpponentManager.Instance.OpponentCardParent.transform);
                Card spawnedCard = cardObject.GetComponent<Card>();
                Vector3 targetPosition = selectedPlayArea.GetSnapPosition(spawnedCard);
                cardObject.transform.DOMove(targetPosition, 0.5f);
                selectedPlayArea.PlaceCard(spawnedCard);
                spawnedCard.Played = true;
                spawnedCard.SetOpponentArea(selectedPlayArea);
                spawnedCard.PlacedOnArea = true;
            }
        }
        else if (card.PlacedOpponentArea == null && card.PlacedArea != null)
        {
            _playAreas = new List<PlayArea>(RoundManager.Instance.PlayerPlayAreas);
            _index = card.PlacedArea.Index;

            List<int> possibleIndices = new List<int> { 1, 2, 3 };
            possibleIndices.Remove(_index);

            System.Random random = new System.Random();
            possibleIndices = possibleIndices.OrderBy(x => random.Next()).ToList();

            PlayArea selectedPlayArea = null;
            foreach (int index in possibleIndices)
            {
                PlayArea playArea = _playAreas[index - 1];
                if (playArea.CheckSnapPointsAvailability())
                {
                    selectedPlayArea = playArea;
                    break;
                }
            }
            if(selectedPlayArea!=null)
            {
                _spawnPoint = DeckManager.Instance.SpawnPosition;
                GameObject cardObject = GameObject.Instantiate(_phantomCard, _spawnPoint.position, Quaternion.identity, DeckManager.Instance.PlayerCardParent.transform);
                Card spawnedCard = cardObject.GetComponent<Card>();
                Vector3 targetPosition = selectedPlayArea.GetSnapPosition(spawnedCard);
                cardObject.transform.DOMove(targetPosition, 0.5f);
                selectedPlayArea.PlaceCard(spawnedCard);
                spawnedCard.Played = true;
                spawnedCard.SetPlayerArea(selectedPlayArea);
                spawnedCard.PlacedOnArea = true;
            }
        }

    }
}
