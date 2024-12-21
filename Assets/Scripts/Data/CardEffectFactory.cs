using System.Collections.Generic;

public static class CardEffectFactory
{
    private static readonly Dictionary<CardEffect, ICardEffect> _cardEffectDictionary = new Dictionary<CardEffect, ICardEffect>
    {
        { CardEffect.None, null },
        { CardEffect.Pioneer, null },
        { CardEffect.Echo, new EchoEffect() },
        { CardEffect.Momentum, new MomentumEffect() },
        { CardEffect.Catalyst, new CatalystEffect() },
        { CardEffect.Winger, new WingerEffect() },
        { CardEffect.Anchor, new AnchorEffect() },
        { CardEffect.Amplifier, new AmplifierEffect() },
        { CardEffect.Scout, new ScoutEffect() },
        { CardEffect.Proxy, new ProxyEffect() }
    };
    public static ICardEffect GetCardEffect(CardEffect cardEffect)
    {
        _cardEffectDictionary.TryGetValue(cardEffect, out ICardEffect effect);
        return effect;
    }
}
