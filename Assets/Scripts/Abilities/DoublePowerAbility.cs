using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoublePowerAbility : ICardAbility
{
    public void ApplyEffect()
    {
        // Bu kart oynand���nda oyun alan�ndaki t�m g��leri iki kat�na ��kar.
        Debug.Log("Double power effect activated");
    }
}
