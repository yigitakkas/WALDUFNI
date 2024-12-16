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

    private GameObject _cardNameObject;
    private GameObject _powerObject;
    private GameObject _energyObject;

    [Header("UI References")]
    private TextMeshProUGUI _cardNameText;
    private TextMeshProUGUI _powerText;
    private TextMeshProUGUI _energyText;

    private Color _originalColor;

    private void Start()
    {
        _cardNameObject = transform.Find("Canvas/CharacterNameText").gameObject;
        _powerObject = transform.Find("Canvas/Damage/DamageText").gameObject;
        _energyObject = transform.Find("Canvas/Energy/EnergyText").gameObject;
        if(_cardNameObject != null && _powerObject != null && _energyObject != null)
        {
            _cardNameText = _cardNameObject.GetComponent<TextMeshProUGUI>();
            _powerText = _powerObject.GetComponent<TextMeshProUGUI>();
            _energyText = _energyObject.GetComponent<TextMeshProUGUI>();
            _originalColor = _powerText.color;
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

    public void IncreasePower(int amount)
    {
        Power += amount;
        if (_powerText != null)
        {
            _powerText.text = Power.ToString();
            HighlightPowerChange();
        }
    }
    public void HighlightPowerChange()
    {
        if (_powerText != null)
        {
            _powerText.color = Color.yellow;
            StartCoroutine(ResetTextColor(_powerText, 0.5f));
        }
    }
    private IEnumerator ResetTextColor(TextMeshProUGUI text, float delay)
    {
        yield return new WaitForSeconds(delay);
        text.color = _originalColor;
    }
}
