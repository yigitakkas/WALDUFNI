using System.Collections.Generic;

public static class CardEffectDescriptions
{
    public static readonly Dictionary<CardEffect, string> Descriptions = new Dictionary<CardEffect, string>
    {
        { CardEffect.None, "No effect" },
        { CardEffect.Pioneer, "Starts in hand at the beginning of the game" },
        { CardEffect.Echo, "Adds another copy of this card to your hand" },
        { CardEffect.Momentum, "Gain +3 Power if you play a card here next turn" },
        { CardEffect.Catalyst, "Gain +2 Power for every card already placed here" },
        { CardEffect.Anchor, "Gain +3 Power if played in the middle area." },
        { CardEffect.Winger, "Gain +2 Power if played in the side areas." },
        { CardEffect.Amplifier, "Doubles the total Power of the area" },
        { CardEffect.Scout, "Sends 1-Power 'Iris' to other areas" },
        { CardEffect.Proxy, "Sends a 6-Power 'Zalor' to a random other area" }
    };
    public static string GetDescription(CardEffect effect)
    {
        if (Descriptions.TryGetValue(effect, out string description))
        {
            return description;
        }
        return "Unknown effect";
    }
}