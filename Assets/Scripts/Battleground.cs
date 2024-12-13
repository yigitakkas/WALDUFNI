using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Battleground : MonoBehaviour
{
    public SpriteRenderer BgImage;
    private bool _activated=false;
    private BattlegroundEffect _battlegroundEffect;
    public TMP_Text Description;
    public TMP_Text Name;
    public int Index;
    public GameObject MonsterCard;
    [SerializeField]
    private Transform _monsterCardSpawnPoint;
    private void Start()
    {
        _monsterCardSpawnPoint = DeckManager.Instance.SpawnPosition.transform;
    }
    public void ActivateBattleground(BattlegroundEffect battlegroundEffect, Sprite sprite, string description, string name)
    {
        BgImage.enabled = true;
        BgImage.sprite = sprite;
        _battlegroundEffect = battlegroundEffect;
        _activated = true;

        Name.text = name;
        Name.gameObject.SetActive(true);
        Description.text = description;
        Description.gameObject.SetActive(true);
    }

    public void ApplyEffect()
    {
        int round = RoundManager.Instance.CurrentRound;
        if (_activated && round == Index)
        {
            switch (_battlegroundEffect)
            {
                case BattlegroundEffect.None:
                    Debug.Log("No effect applied.");
                    break;

                case BattlegroundEffect.BeastLair:
                    ApplyBeastLairEffect();
                    break;

                case BattlegroundEffect.TheApexZone:
                    ApplyTheApexZoneEffect();
                    break;

                case BattlegroundEffect.FieldOfGrowth:
                    ApplyFieldOfGrowthEffect();
                    break;

                case BattlegroundEffect.ForgeOfMight:
                    ApplyForgeOfMightEffect();
                    break;

                case BattlegroundEffect.ControlZone:
                    ApplyControlZoneEffect();
                    break;
                default:
                    Debug.LogError("Unknown BattlegroundEffect!");
                    break;
            }
        }
    }

    private void ApplyBeastLairEffect()
    {
        foreach(PlayArea playArea in RoundManager.Instance.PlayerPlayAreas)
        {
            if(playArea.Index == Index && playArea.PlayedHereThisRound && playArea.CheckSnapPointsAvailability())
            {
                Debug.Log("Indexler ayný ve Played Here");
                GameObject spawnedCardObject = Instantiate(MonsterCard, _monsterCardSpawnPoint.position, Quaternion.identity);
                spawnedCardObject.transform.SetParent(playArea.transform);
                Card spawnedCard = spawnedCardObject.GetComponent<Card>();

                Vector3 targetPosition = playArea.GetSnapPosition(spawnedCard);
                spawnedCardObject.transform.DOMove(targetPosition, 0.5f);
                playArea.PlaceCard(spawnedCard);
                spawnedCard.Played = true;
                spawnedCard.SetPlayerArea(playArea);
            }
        }
        foreach (PlayArea playArea in RoundManager.Instance.OpponentPlayAreas)
        {
            if (playArea.Index == Index && playArea.PlayedHereThisRound)
            {
                Debug.Log("Indexler ayný ve Played Here // Opponent");
                GameObject spawnedCardObject = Instantiate(MonsterCard, _monsterCardSpawnPoint.position, Quaternion.identity);
                spawnedCardObject.transform.SetParent(playArea.transform);
                Card spawnedCard = spawnedCardObject.GetComponent<Card>();

                Vector3 targetPosition = playArea.GetSnapPosition(spawnedCard);
                spawnedCardObject.transform.DOMove(targetPosition, 0.5f);
                playArea.PlaceCard(spawnedCard);
                spawnedCard.Played = true;
                spawnedCard.SetOpponentArea(playArea);
            }
        }
    }

    private void ApplyTheApexZoneEffect()
    {

    }
    private void ApplyFieldOfGrowthEffect()
    {

    }
    private void ApplyForgeOfMightEffect()
    {

    }
    private void ApplyControlZoneEffect()
    {

    }
}
