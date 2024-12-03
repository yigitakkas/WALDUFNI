using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoublePowerAbility : ICardAbility
{
    public void ApplyEffect()
    {
        // Bu kart oynandýðýnda oyun alanýndaki tüm güçleri iki katýna çýkar.
        Debug.Log("Double power effect activated");
    }
}
