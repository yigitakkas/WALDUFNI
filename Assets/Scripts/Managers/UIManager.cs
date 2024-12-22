using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public GameObject PopUpPanel;
    public TMP_Text WonText;
    public Button NextLevelButton;
    public Button TryAgainButton;
    public GameObject BlockerPanel;
    public Button PlayButton;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnEnable()
    {
        RoundManager.OnRoundStarted += DeactivateButton;
        RoundManager.OnRoundEnded += ActivateButton;
    }

    private void OnDisable()
    {
        RoundManager.OnRoundStarted -= DeactivateButton;
        RoundManager.OnRoundEnded -= ActivateButton;
    }
    public void ShowPopup(string message, bool isPlayerWinner)
    {
        BlockerPanel.SetActive(true);
        PopUpPanel.SetActive(true);

        if (isPlayerWinner)
        {
            WonText.color = Color.blue;
            NextLevelButton.gameObject.SetActive(true);
        }
        else
        {
            WonText.color = Color.red;
            TryAgainButton.gameObject.SetActive(true);
        }

        WonText.text = message;
    }

    public void HidePopup()
    {
        WonText.text = "";
        WonText.color = Color.white;
        BlockerPanel.SetActive(false);
        PopUpPanel.SetActive(false);
        NextLevelButton.gameObject.SetActive(false);
        TryAgainButton.gameObject.SetActive(false);
    }

    private void ActivateButton()
    {
        PlayButton.interactable = true;
    }
    private void DeactivateButton()
    {
        PlayButton.interactable = false;
    }
}
