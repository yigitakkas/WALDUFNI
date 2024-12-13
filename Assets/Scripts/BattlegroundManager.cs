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
    private Dictionary<BattlegroundEffect, string> _effectDescriptionDictionary;
    private void Awake()
    {
        InitializeDictionaries();
        FindBattlegrounds();
        UnlockBattleground(0);
    }

    private void InitializeDictionaries()
    {
        _effectSpriteDictionary = new Dictionary<BattlegroundEffect, Sprite>
        {
            { BattlegroundEffect.BeastLair, BeastLairSprite },
            { BattlegroundEffect.TheApexZone, TheApexZoneSprite },
            { BattlegroundEffect.FieldOfGrowth, FieldOfGrowthSprite },
            { BattlegroundEffect.ForgeOfMight, ForgeOfMightSprite },
            { BattlegroundEffect.ControlZone, ControlZoneSprite }
        };

        _effectDescriptionDictionary = new Dictionary<BattlegroundEffect, string>
        {
        { BattlegroundEffect.BeastLair, "Get a 10-Power monster card if you play a card here this turn." },
        { BattlegroundEffect.TheApexZone, "The strongest card(s) in this area gains +3 power." },
        { BattlegroundEffect.FieldOfGrowth, "All cards gain +1 Power in this magical zone." },
        { BattlegroundEffect.ForgeOfMight, "Each card played here gains +2 Power." },
        { BattlegroundEffect.ControlZone, "Player with most cards here gains +100 Power." }
        };
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
    private void UnlockBattleground(int index)
    {
        BattlegroundEffect randomEffect = GetRandomBattlegroundEffect();
        Sprite sprite = GetSpriteForEffect(randomEffect);
        string description = GetStringForEffect(randomEffect);
        string name = FormatEnumName(randomEffect);

        _battlegrounds[index].ActivateBattleground(randomEffect, sprite, description, name);
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

    public string GetStringForEffect(BattlegroundEffect effect)
    {
        if(_effectDescriptionDictionary.TryGetValue(effect, out string description))
        {
            return description;
        }

        Debug.LogWarning($"No description found for effect: {effect}");
        return null;
    }
    public static string FormatEnumName(BattlegroundEffect effect)
    {
        string name = effect.ToString();
        return System.Text.RegularExpressions.Regex.Replace(name, "([a-z])([A-Z])", "$1 $2").ToUpper();
    }
}

public enum BattlegroundEffect
{
    None,
    BeastLair,   // Adds 10 Power monster cards for both players
    TheApexZone,     // Strongest card in this area gains +3 Power
    FieldOfGrowth,   // All cards in this area receive +1 Power
    ForgeOfMight,   // Adds +2 Power to any card played in this area
    ControlZone  // Player with the most cards here gains +100 power
}