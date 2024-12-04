using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    [Header("Card Attributes")]
    public string CharacterName;
    public int Energy;
    public int Power;

    private Transform _cardNameObject;
    private Transform _powerObject;
    private Transform _energyObject;

    [Header("UI References")]
    private TextMeshProUGUI _cardNameText;
    private TextMeshProUGUI _powerText;
    private TextMeshProUGUI _energyText;

    private void Start()
    {
        _cardNameObject = transform.Find("CharacterNameText");
        _powerObject = transform.Find("DamageText");
        _energyObject = transform.Find("EnergyText");
        if(_cardNameObject != null && _powerObject != null && _energyObject != null)
        {
            _cardNameText = _cardNameObject.GetComponent<TextMeshProUGUI>();
            _powerText = _powerObject.GetComponent<TextMeshProUGUI>();
            _energyText = _energyObject.GetComponent<TextMeshProUGUI>();
        }
        UpdateCardDisplay();
    }

    public void UpdateCardDisplay()
    {
        if (_cardNameText != null)
        {
            _cardNameText.text = CharacterName;
        }

        if (_powerText != null)
        {
            _powerText.text = Power.ToString();
        }

        if (_energyText != null)
        {
            _energyText.text = Energy.ToString();
        }
    }
}
