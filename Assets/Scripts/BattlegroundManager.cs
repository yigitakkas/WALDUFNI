using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BattlegroundManager : MonoBehaviour
{
    public Sprite BeastLairSprite;
    public Sprite TheApexZoneSprite;
    public Sprite FieldOfGrowthSprite;
    public Sprite ForgeOfMightSprite;
    public Sprite ControlZoneSprite;
    [SerializeField]
    private List<Battleground> _battlegrounds = new List<Battleground>();
    private Dictionary<BattlegroundEffect, Sprite> _effectSpriteDictionary;
    private void Awake()
    {
        _effectSpriteDictionary = new Dictionary<BattlegroundEffect, Sprite>
        {
            { BattlegroundEffect.BeastLair, BeastLairSprite },
            { BattlegroundEffect.TheApexZone, TheApexZoneSprite },
            { BattlegroundEffect.FieldOfGrowth, FieldOfGrowthSprite },
            { BattlegroundEffect.ForgeOfMight, ForgeOfMightSprite },
            { BattlegroundEffect.ControlZone, ControlZoneSprite }
        };

        FindBattlegrounds();
        UnlockFirstBattleground();
    }

    private void FindBattlegrounds()
    {
        Battleground[] battlegrounds = GetComponentsInChildren<Battleground>();
        foreach (Battleground battleground in battlegrounds)
        {
            if (battleground.CompareTag("Battleground"))
                _battlegrounds.Add(battleground);
        }
    }
    private void UnlockFirstBattleground()
    {
        BattlegroundEffect randomEffect = GetRandomBattlegroundEffect();
        _battlegrounds[0].BgImage.enabled = true;
        _battlegrounds[0].BgImage.sprite = GetSpriteForEffect(randomEffect);
    }

    private BattlegroundEffect GetRandomBattlegroundEffect()
    {
        BattlegroundEffect[] effects = (BattlegroundEffect[])System.Enum.GetValues(typeof(BattlegroundEffect));

        int randomIndex = UnityEngine.Random.Range(1, effects.Length); // 1'den baþlar çünkü 0 = None

        return effects[randomIndex];
    }

    public Sprite GetSpriteForEffect(BattlegroundEffect effect)
    {
        if (_effectSpriteDictionary.TryGetValue(effect, out Sprite sprite))
        {
            return sprite;
        }

        Debug.LogWarning($"No sprite found for effect: {effect}");
        return null;
    }

    public enum BattlegroundEffect
    {
        BeastLair,   // Adds 10 Power monster cards for both players
        TheApexZone,     // Strongest card in this area gains +3 Power
        FieldOfGrowth,   // All cards in this area receive +1 Power
        ForgeOfMight,   // Adds +2 Power to any card played in this area
        ControlZone  // Player with the most cards here gains +100 power
    }
}
