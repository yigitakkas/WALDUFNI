using System.Collections.Generic;

public static class CardEffectFactory
{
    private static readonly Dictionary<CardEffect, ICardEffect> _cardEffectDictionary = new Dictionary<CardEffect, ICardEffect>
    {
        { CardEffect.None, null },
        { CardEffect.Pioneer, new PioneerEffect() },
        { CardEffect.Echo, new EchoEffect() },
        { CardEffect.Momentum, new MomentumEffect() },
        { CardEffect.Catalyst, new CatalystEffect() },
        { CardEffect.TriggerLeft, new TriggerLeftEffect() },
        { CardEffect.TriggerMiddle, new TriggerMiddleEffect() },
        { CardEffect.TriggerRight, new TriggerRightEffect() },
        { CardEffect.Amplifier, new AmplifierEffect() }
    };
    public static ICardEffect GetCardEffect(CardEffect cardEffect)
    {
        _cardEffectDictionary.TryGetValue(cardEffect, out ICardEffect effect);
        return effect;
    }
}
