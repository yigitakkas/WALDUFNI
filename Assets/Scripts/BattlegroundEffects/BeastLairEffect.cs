using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BeastLairEffect : IBattlegroundEffect
{
    public void ApplyEffect(Battleground battleground)
    {
        if(RoundManager.Instance.CurrentRound == battleground.Index)
        {
            CreateAndPlaceMonsterCard(battleground,isPlayer: true);
            CreateAndPlaceMonsterCard(battleground,isPlayer: false);
            battleground.ChangeColorOverTime(new Color(25f / 255f, 25f / 255f, 25f / 255f, battleground.BgImage.color.a), 1f);
        }
    }
    private void CreateAndPlaceMonsterCard(Battleground battleground, bool isPlayer)
    {
        PlayArea playArea = RoundManager.Instance.GetAreaWithIndex(battleground.Index, isPlayer);

        if (playArea.PlayedHereThisRound && playArea.CheckSnapPointsAvailability())
        {
            GameObject monsterCard = battleground.MonsterCard;
            Transform spawnPoint;
            if (isPlayer)
                spawnPoint = battleground.MonsterCardPlayerSpawnPoint;
            else spawnPoint = battleground.MonsterCardOpponentSpawnPoint;

            // Kart oluþtur ve pozisyonla
            GameObject cardObject = GameObject.Instantiate(monsterCard, spawnPoint.position, Quaternion.identity);
            Card spawnedCard = cardObject.GetComponent<Card>();

            Vector3 targetPosition = playArea.GetSnapPosition(spawnedCard);
            cardObject.transform.DOMove(targetPosition, 0.5f);

            // Kartý alana yerleþtir ve özelliklerini ayarla
            playArea.PlaceCard(spawnedCard);
            spawnedCard.Played = true;
            spawnedCard.PlacedOnArea = true;

            if (isPlayer)
            {
                spawnedCard.SetPlayerArea(playArea);
                spawnedCard.transform.SetParent(DeckManager.Instance.PlayerCardParent.transform);
            }
            else
            {
                spawnedCard.SetOpponentArea(playArea);
                spawnedCard.transform.SetParent(OpponentManager.Instance.OpponentCardParent.transform);
            }
        }
    }
}
