using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using System.Text.RegularExpressions;


public class BattlegroundManager : MonoBehaviour
{
    public static BattlegroundManager Instance;

    public Sprite BeastLairSprite;
    public Sprite TheApexZoneSprite;
    public Sprite FieldOfGrowthSprite;
    public Sprite ForgeOfMightSprite;
    public Sprite ControlZoneSprite;

    [SerializeField]
    private List<Battleground> _battlegrounds = new List<Battleground>();

    private Dictionary<BattlegroundEffect, Sprite> _effectSpriteDictionary;
    private Dictionary<BattlegroundEffect, string> _effectDescriptionDictionary;
    private List<BattlegroundEffect> _usedEffects = new List<BattlegroundEffect>();

    private void OnEnable()
    {
        RoundManager.OnRoundEnded += ActivateNewBattleground;
    }

    private void OnDisable()
    {
        RoundManager.OnRoundEnded -= ActivateNewBattleground;
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        InitializeDictionaries();
        FindBattlegrounds();
        UnlockBattleground(0);
    }
    private void ActivateNewBattleground()
    {
        int round = RoundManager.Instance.CurrentRound;
        if(round == 2)
        {
            UnlockBattleground(1);
        } 
        else if(round == 3)
        {
            UnlockBattleground(2);
        }
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
            { BattlegroundEffect.BeastLair, "Get a 10-Power Umbra\n if you play a card here this turn." },
            { BattlegroundEffect.TheApexZone, "The strongest card(s) in this area\n gains 3 Power." },
            { BattlegroundEffect.FieldOfGrowth, "Each card played here\n gains 1 Power." },
            { BattlegroundEffect.ForgeOfMight, "Each card played here\n gains 2 Power." },
            { BattlegroundEffect.ControlZone, "Player with most cards here\n gains 100 Power." }
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
        //BattlegroundEffect randomEffect = BattlegroundEffect.FieldOfGrowth;
        Sprite sprite = GetSpriteForEffect(randomEffect);
        string description = GetStringForEffect(randomEffect);
        string name = FormatEnumName(randomEffect);

        _battlegrounds[index].ActivateBattleground(randomEffect, sprite, description, name);
    }

    private BattlegroundEffect GetRandomBattlegroundEffect()
    {
        BattlegroundEffect[] effects = (BattlegroundEffect[])Enum.GetValues(typeof(BattlegroundEffect));
        List<BattlegroundEffect> availableEffects = new List<BattlegroundEffect>();
        foreach (BattlegroundEffect effect in effects)
        {
            if (effect != BattlegroundEffect.None && !_usedEffects.Contains(effect))
            {
                availableEffects.Add(effect);
            }
        }
        if (availableEffects.Count == 0)
        {
            Debug.LogWarning("Tüm BattlegroundEffect'ler kullanýldý.");
            return BattlegroundEffect.None;
        }
        int randomIndex = UnityEngine.Random.Range(0, availableEffects.Count);
        BattlegroundEffect selectedEffect = availableEffects[randomIndex];
        _usedEffects.Add(selectedEffect);
        return selectedEffect;
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
        return Regex.Replace(name, "([a-z])([A-Z])", "$1 $2").ToUpper(CultureInfo.InvariantCulture);
    }

    public void ApplyBattlegroundEffects()
    {
        foreach(Battleground battleground in _battlegrounds)
        {
            battleground.ApplyEffect();
        }
    }
}

public enum BattlegroundEffect
{
    None,
    BeastLair,     // Get a 10-Power monster card if you play a card here this turn.
    TheApexZone,   // The strongest card(s) in this area gains +3 power.
    FieldOfGrowth, // All cards gain +1 Power in this magical zone.
    ForgeOfMight,  // Each card played here gains +2 Power.
    ControlZone    // Player with most cards here gains +100 Power.
}