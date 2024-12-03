using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    [Header("Card Attributes")]
    public string cardName;
    public int energy;
    public int power;

    [Header("UI References")]
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI powerText;
    public TextMeshProUGUI energyText;

    private void Start()
    {
        UpdateCardDisplay();
    }

    public void UpdateCardDisplay()
    {
        if (cardNameText != null)
        {
            cardNameText.text = cardName;
        }

        if (powerText != null)
        {
            powerText.text = power.ToString();
        }

        if (energyText != null)
        {
            energyText.text = energy.ToString();
        }
    }
}
