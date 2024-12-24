using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public GameObject tutorialPanel;
    public TMP_Text tutorialText;
    public Button nextButton;
    private GameObject _currentHighlight;

    public GameObject PlayerHandHiglight;
    public GameObject EnergyBarHiglight;
    public GameObject RoundBarHiglight;
    public GameObject PlayButtonHiglight;
    public GameObject BattlegroundHighlight;

    private int currentStep = 0;

    private string[] tutorialSteps = {
        "Welcome to the game! Let's learn how to play.",
        "This is your hand. You can hover over a card to see its abilities.",
        "Cards require 'Energy' to play. Drag one of your cards to a 'Play Area'.",
        "You can see the current round from here. You'll gain 'Energy' each round.",
        "These are 'Battlegrounds'. Each of these will have different effects.",
        "All set! Now let's win by dominating the battlegrounds with the highest total power!"
    };

    private void Start()
    {
        if (PlayerPrefs.GetInt("TutorialCompleted", 0) == 0)
        {
            StartTutorial();
        }
        else
        {
            Debug.Log("Tutorial zaten tamamlanmýþ.");
        }
    }

    private void StartTutorial()
    {
        tutorialPanel.SetActive(true);
        ShowTutorialStep();           
    }

    public void NextStep()
    {
        currentStep++;
        if (currentStep < tutorialSteps.Length)
        {
            RemoveHighlight();
            ShowTutorialStep();
        }
        else
        {
            EndTutorial();
        }
    }

    private void ShowTutorialStep()
    {
        tutorialText.text = tutorialSteps[currentStep]; 

        switch (currentStep)
        {
            case 1:
                HighlightObject("PlayerHand");
                break;
            case 2:
                HighlightObject("EnergyBar");
                break;
            case 3:
                HighlightObject("RoundBar");
                break;
            case 4:
                HighlightObject("Battleground");
                break;
            case 5:
                HighlightObject("PlayButton");
                break;
            default:
                RemoveHighlight();
                break;
        }
    }

    private void HighlightObject(string objectName)
    {
        if (objectName == "PlayerHand")
            _currentHighlight = PlayerHandHiglight;
        else if (objectName == "EnergyBar")
            _currentHighlight = EnergyBarHiglight;
        else if (objectName == "RoundBar")
            _currentHighlight = RoundBarHiglight;
        else if (objectName == "Battleground")
            _currentHighlight = BattlegroundHighlight;
        else if (objectName == "PlayButton")
            _currentHighlight = PlayButtonHiglight;

        if (_currentHighlight != null)
        {
            _currentHighlight.SetActive(true);
        }
    }

    private void RemoveHighlight()
    {
        if(_currentHighlight!=null)
            _currentHighlight.SetActive(false);
    }

    private void EndTutorial()
    {
        tutorialPanel.SetActive(false); // Tutorial panelini gizle
        RemoveHighlight();
        PlayerPrefs.SetInt("TutorialCompleted", 1); // Tutorial tamamlandý olarak iþaretle
        Debug.Log("Tutorial tamamlandý.");
    }
}
