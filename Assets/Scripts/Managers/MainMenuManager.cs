using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public Button PlayButton;
    public Button CreditsButton;
    public Button BackButton;
    public GameObject MenuPanel;
    public GameObject CreditsPanel;

    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    public void Credits()
    {
        MenuPanel.SetActive(false);
        CreditsPanel.SetActive(true);
    }

    public void BackToMenu()
    {
        CreditsPanel.SetActive(false);
        MenuPanel.SetActive(true);
    }

}
