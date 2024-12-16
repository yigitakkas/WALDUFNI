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
            battleground.BgImage.color = new Color(25f / 255f, 25f / 255f, 25f / 255f, battleground.BgImage.color.a);
        }
    }
    private void CreateAndPlaceMonsterCard(Battleground battleground, bool isPlayer)
    {
        PlayArea playArea = RoundManager.Instance.GetAreaWithIndex(battleground.Index, isPlayer);

        if (playArea.PlayedHereThisRound)
        {
            GameObject monsterCard = battleground.MonsterCard;
            Transform spawnPoint = battleground.MonsterCardSpawnPoint;

            // Kart oluþtur ve pozisyonla
            GameObject cardObject = GameObject.Instantiate(monsterCard, spawnPoint.position, Quaternion.identity);
            cardObject.transform.SetParent(playArea.transform);
            Card spawnedCard = cardObject.GetComponent<Card>();

            Vector3 targetPosition = playArea.GetSnapPosition(spawnedCard);
            cardObject.transform.DOMove(targetPosition, 0.5f);

            // Kartý alana yerleþtir ve özelliklerini ayarla
            playArea.PlaceCard(spawnedCard);
            spawnedCard.Played = true;

            if (isPlayer)
            {
                spawnedCard.SetPlayerArea(playArea);
            }
            else
            {
                spawnedCard.SetOpponentArea(playArea);
            }
        }
    }
}
