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
    private Transform _monsterCardPlayerSpawnPoint;
    public Transform MonsterCardPlayerSpawnPoint
    {
        get => _monsterCardPlayerSpawnPoint;
        private set => _monsterCardPlayerSpawnPoint = value;
    }
    private Transform _monsterCardOpponentSpawnPoint;
    public Transform MonsterCardOpponentSpawnPoint
    {
        get => _monsterCardOpponentSpawnPoint;
        private set => _monsterCardOpponentSpawnPoint = value;
    }

    public GameObject MonsterCard;


    public HashSet<Card> AllCards = new HashSet<Card>();
    public List<Card> AllCardsList = new List<Card>();

    private Color _targetColor = new Color32(255, 255, 255, 255);
    private float _colorChangeDuration = 0.5f;

    private void Start()
    {
        _monsterCardPlayerSpawnPoint = DeckManager.Instance.SpawnPosition.transform;
        _monsterCardOpponentSpawnPoint = OpponentManager.Instance.SpawnPosition.transform;
    }
    public void ActivateBattleground(BattlegroundEffect battlegroundEffect, Sprite sprite, string description, string name)
    {
        BgImage.sprite = sprite;
        ChangeColorOverTime(_targetColor, _colorChangeDuration);
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
        AllCardsList = new List<Card>(AllCards); // Inspector'da g�r�nt�leme
    }

    public void ChangeColorOverTime(Color targetColor, float duration)
    {
        BgImage.DOColor(targetColor, duration);
    }
}