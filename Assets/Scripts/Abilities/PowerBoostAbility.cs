using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerBoostAbility : ICardAbility
{
    private int _boostAmount;

    public PowerBoostAbility(int boostAmount)
    {
        _boostAmount = boostAmount;
    }

    public void ApplyEffect()
    {
        // Power Boost logic, such as increasing the power of the card or other conditions.
        Debug.Log("Power boosted by" + _boostAmount);

    }
}