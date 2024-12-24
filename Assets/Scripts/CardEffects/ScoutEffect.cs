using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScoutEffect : ICardEffect
{
    private List<PlayArea> _playAreas = new List<PlayArea>();
    int _index = 0;
    GameObject _spotterCard;
    Transform _spawnPoint;
    public void ApplyEffect(Card card)
    {
        //Sends 1-power 2 cards to other areas — On reveal
        _spotterCard = DeckManager.Instance.SpotterCard;
        
        _playAreas.Clear();

        if (card.PlacedOpponentArea != null)
        {
            _playAreas = new List<PlayArea>(RoundManager.Instance.OpponentPlayAreas);
            _index = card.PlacedOpponentArea.Index;
            for (int i = 1; i <= 3; i++)
            {
                if (i != _index && _playAreas[i - 1].CheckSnapPointsAvailability())
                {
                    _spawnPoint = OpponentManager.Instance.SpawnPosition;
                    GameObject cardObject = GameObject.Instantiate(_spotterCard, _spawnPoint.position, Quaternion.identity, OpponentManager.Instance.OpponentCardParent.transform);
                    Card spawnedCard = cardObject.GetComponent<Card>();
                    Vector3 targetPosition = _playAreas[i - 1].GetSnapPosition(spawnedCard);
                    cardObject.transform.DOMove(targetPosition, 0.5f);
                    _playAreas[i - 1].PlaceCard(spawnedCard);
                    spawnedCard.Played = true;
                    spawnedCard.SetOpponentArea(_playAreas[i - 1]);
                    spawnedCard.PlacedOnArea = true;
                }
            }
        }
        else if (card.PlacedOpponentArea == null && card.PlacedArea != null)
        {
            _playAreas = new List<PlayArea>(RoundManager.Instance.PlayerPlayAreas);
            _index = card.PlacedArea.Index;
            for (int i = 1; i <= 3; i++)
            {
                if (i != _index && _playAreas[i - 1].CheckSnapPointsAvailability())
                {
                    _spawnPoint = DeckManager.Instance.SpawnPosition;
                    GameObject cardObject = GameObject.Instantiate(_spotterCard, _spawnPoint.position, Quaternion.identity, DeckManager.Instance.PlayerCardParent.transform);
                    Card spawnedCard = cardObject.GetComponent<Card>();
                    Vector3 targetPosition = _playAreas[i - 1].GetSnapPosition(spawnedCard);
                    cardObject.transform.DOMove(targetPosition, 0.5f);
                    _playAreas[i - 1].PlaceCard(spawnedCard);
                    spawnedCard.Played = true;
                    spawnedCard.SetPlayerArea(_playAreas[i - 1]);
                    spawnedCard.PlacedOnArea = true;
                    SoundManager.Instance.PlaySFX(SoundManager.Instance.CardPlayedSound);
                }
            }
        }

    }
}
