using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnergyManager : MonoBehaviour
{
    public static EnergyManager Instance;
    public TMP_Text PlayerEnergyText;

    public int PlayerEnergy { get; private set; }
    public int OpponentEnergy { get; private set; }
    private void OnEnable()
    {
        RoundManager.OnRoundEnded += SetEnergy;
    }

    private void OnDisable()
    {
        RoundManager.OnRoundEnded -= SetEnergy;
    }
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        ResetEnergy();
    }

    private void ResetEnergy()
    {
        PlayerEnergy = 1;
        OpponentEnergy = 1;
        SetText();
    }

    public void SetEnergy()
    {
        int index = RoundManager.Instance.CurrentRound;
        PlayerEnergy = index;
        OpponentEnergy = index;
        SetText();
        DeckManager.Instance.SetDarknessOfCards(PlayerEnergy);
    }

    private void SetText()
    {
        PlayerEnergyText.text = PlayerEnergy.ToString();
    }

    public bool CheckIfMovable(int energy, Card card)
    {
        if (card.PlacedArea != null) return true;
        if (energy > PlayerEnergy)
            return false;
        else return true;
    }

    public void DecreaseEnergy(int energy, bool player)
    {
        if(player)
        {
            PlayerEnergy -= energy;
            SetText();
            DeckManager.Instance.SetDarknessOfCards(PlayerEnergy);
        }
        else
        {
            OpponentEnergy -= energy;
        }
    }
    public void IncreaseEnergy(int energy, bool player)
    {
        PlayerEnergy += energy;
        SetText();
        DeckManager.Instance.SetDarknessOfCards(PlayerEnergy);
    }


}
