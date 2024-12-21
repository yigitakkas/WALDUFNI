using System.Collections;
using UnityEngine;
using TMPro;

public partial class Card
{
    private void CardDisplayAwake()
    {
        _cardNameObject = transform.Find("CardVisual/Canvas/CharacterNameText").gameObject;
        _powerObject = transform.Find("CardVisual/Canvas/Damage/DamageText").gameObject;
        _energyObject = transform.Find("CardVisual/Canvas/Energy/EnergyText").gameObject;
        _overlayObject = transform.Find("CardVisual/Overlay").gameObject;

        if (_cardNameObject != null && _powerObject != null && _energyObject != null)
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
            StartCoroutine(ResetTextColor(_powerText, 0.5f));
        }
    }
    private IEnumerator ResetTextColor(TextMeshProUGUI text, float delay)
    {
        yield return new WaitForSeconds(delay);
        StartHoverEffect();
        _powerText.text = Power.ToString();
        _powerText.color = Color.yellow;
        yield return new WaitForSeconds(delay);
        text.color = _originalColor;
    }

    public void DarkenObject()
    {
        if(!CheckIfPlaced())
        {
            _overlayObject.SetActive(true);
        }
    }

    public void LightenObject()
    {
        if (!CheckIfPlaced())
        {
            _overlayObject.SetActive(false);
        }
    }

    private bool CheckIfPlaced()
    {
        return PlacedOnArea;
    }
}
