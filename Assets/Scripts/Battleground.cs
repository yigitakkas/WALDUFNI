using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Battleground : MonoBehaviour
{
    private static readonly Dictionary<BattlegroundEffect, IBattlegroundEffect> EffectDictionary =
    new Dictionary<BattlegroundEffect, IBattlegroundEffect>
    {
        { BattlegroundEffect.BeastLair, new BeastLairEffect() },
        { BattlegroundEffect.TheApexZone, new ApexZoneEffect() },
        { BattlegroundEffect.FieldOfGrowth, new FieldOfGrowthEffect() },
        { BattlegroundEffect.ForgeOfMight, new ForgeOfMightEffect() },
        { BattlegroundEffect.ControlZone, new ControlZoneEffect() }
    };


    public SpriteRenderer BgImage;
    public TMP_Text Description;
    public TMP_Text Name;
    public int Index;

    [SerializeField]
    private BattlegroundEffect _battlegroundEffect;

    private bool _activated = false;

    public BattlegroundEffect BattlegroundEffect
    {
        get => _battlegroundEffect;
        private set => _battlegroundEffect = value;
    }
    private Transform _monsterCardSpawnPoint;
    public Transform MonsterCardSpawnPoint
    {
        get => _monsterCardSpawnPoint;
        private set => _monsterCardSpawnPoint = value;
    }
    public GameObject MonsterCard;


    public HashSet<Card> AllCards = new HashSet<Card>();
    public List<Card> AllCardsList = new List<Card>();

    private Color _targetColor = new Color32(255, 255, 255, 255);
    private float _colorChangeDuration = 0.5f;

    private void Start()
    {
        _monsterCardSpawnPoint = DeckManager.Instance.SpawnPosition.transform;
    }
    public void ActivateBattleground(BattlegroundEffect battlegroundEffect, Sprite sprite, string description, string name)
    {
        BgImage.sprite = sprite;
        BgImage.DOColor(_targetColor, _colorChangeDuration);
        BattlegroundEffect = battlegroundEffect;
        _activated = true;

        Name.text = name;
        Description.text = description;
    }

    public void ApplyEffect()
    {
        if (!_activated) return;

        if (EffectDictionary.TryGetValue(_battlegroundEffect, out IBattlegroundEffect effect))
        {
            effect.ApplyEffect(this);
        }
    }

    public void UpdateAllCards()
    {
        PlayArea player = RoundManager.Instance.GetAreaWithIndex(Index, true);
        PlayArea opponent = RoundManager.Instance.GetAreaWithIndex(Index, false);

        AllCards.UnionWith(player.PlacedCards);
        AllCards.UnionWith(opponent.PlacedCards);
        AllCardsList = new List<Card>(AllCards); // Inspector'da görüntüleme
    }
    private void ApplyForgeOfMightEffect()
    {
        //her tur eklenen kart(lar)a +2 power verecek, Daha önce power alan kart tekrar +1 almayacak.
    }
    private void ApplyControlZoneEffect()
    {
        //her tur tüm kartlarý kontrol edip en fazla kartý olan tarafýn skoruna +100 ekleyecek. +100 tek bir tarafa olmalý ve tek bir defa eklenmeli, eþitse iki tarafa da eklenmeli
    }
}