public enum CardEffect
{
    None,
    Pioneer, //Starts at hand
    Echo, //Add another copy of this card to hand
    Momentum, //Gain +3 power if you play a card here next turn
    Catalyst, //Gain +2 power for every card placed here
    TriggerLeft, //Gain +3 power if played in left area
    TriggerMiddle, //Gain +3 power if played in middle area
    TriggerRight, //Gain +3 power if played in right area
    Amplifier, //Doubles the total power of the area
    Scout, //Sends 1-power 2 cards to other areas
    Proxy //Sends 6-power card to random other area
}