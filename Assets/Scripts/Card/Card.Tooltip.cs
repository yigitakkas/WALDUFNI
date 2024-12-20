using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Text.RegularExpressions;

public partial class Card
{
    private void ShowCardTooltip()
    {
        if (_currentTooltip != null) return;

        _currentTooltip = Instantiate(CardTooltipPrefab, _cardVisual.transform);
        _currentTooltip.transform.localPosition = _tooltipOffset;

        TextMeshProUGUI tooltipText = _currentTooltip.GetComponentInChildren<TextMeshProUGUI>();
        if (tooltipText != null)
        {
            string description = CardEffectDescriptions.GetDescription(CardEffectType);
            tooltipText.text = $"{AddSpacesToPascalCase(CardTriggerType.ToString())}\n\n" +
                   $"{description}";
        }
    }

    private void HideCardTooltip()
    {
        if (_currentTooltip != null)
        {
            Destroy(_currentTooltip);
            _currentTooltip = null;
        }
    }

    public string AddSpacesToPascalCase(string text)
    {
        return Regex.Replace(text, "(\\B[A-Z])", " $1");
    }
}
