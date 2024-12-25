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
    public GameObject BlockerPanel;

    public Button PlayButton;

    public TMP_Text AreaOnePlayerScore;
    public TMP_Text AreaTwoPlayerScore;
    public TMP_Text AreaThreePlayerScore;
    public TMP_Text AreaOneOpponentScore;
    public TMP_Text AreaTwoOpponentScore;
    public TMP_Text AreaThreeOpponentScore; 
    public TMP_Text PlayerEnergyText;
    public TMP_Text RoundText;

    public Canvas canvas;

    private GameObject _playAgainButton;
    private GameObject _mainMenuButton;

    public Button MusicButton;
    public Sprite MusicOnSprite;
    public Sprite MusicOffSprite;
    private Image _buttonImage;

    public GameObject MainMenuPopupPanel;
    public Button MainMenuShortcutButton;


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
        _playAgainButton = PopUpPanel.transform.Find("PlayAgainButton").gameObject;
        _mainMenuButton = PopUpPanel.transform.Find("MainMenuButton").gameObject;
        _buttonImage = MusicButton.GetComponent<Image>();
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
    public IEnumerator ShowPopup(string message, bool isPlayerWinner)
    {
        yield return StartCoroutine(GiveEffectToScores());

        WonText.gameObject.SetActive(true);
        _playAgainButton.SetActive(false);
        _mainMenuButton.SetActive(false);

        BlockerPanel.SetActive(true);
        PopUpPanel.SetActive(true);

        if (isPlayerWinner)
        {
            WonText.color = Color.blue;
            SoundManager.Instance.PlaySFX(SoundManager.Instance.PlayerWonSound);
        }
        else
        {
            WonText.color = Color.red;
            SoundManager.Instance.PlaySFX(SoundManager.Instance.OpponentWonSound);
        }

        WonText.text = message;

        yield return new WaitForSeconds(2f);
        WonText.gameObject.SetActive(false);
        _playAgainButton.SetActive(true);
        _mainMenuButton.SetActive(true);
    }

    public void HidePopup()
    {
        WonText.text = "";
        WonText.color = Color.white;
        BlockerPanel.SetActive(false);
        PopUpPanel.SetActive(false);
    }

    private void ActivateButton()
    {
        PlayButton.interactable = true;
        UpdateRound();

    }
    private void DeactivateButton()
    {
        PlayButton.interactable = false;
    }
    public void UpdateUI()
    {
        Dictionary<int, int> playerScores = ScoreManager.Instance.PlayerScores;
        Dictionary<int, int> opponentScores = ScoreManager.Instance.OpponentScores;

        AreaOnePlayerScore.text = playerScores[1].ToString();
        AreaTwoPlayerScore.text = playerScores[2].ToString();
        AreaThreePlayerScore.text = playerScores[3].ToString();

        AreaOneOpponentScore.text = opponentScores[1].ToString();
        AreaTwoOpponentScore.text = opponentScores[2].ToString();
        AreaThreeOpponentScore.text = opponentScores[3].ToString();

        CompareAndSetTextColor(AreaOnePlayerScore, AreaOneOpponentScore, playerScores[1], opponentScores[1]);
        CompareAndSetTextColor(AreaTwoPlayerScore, AreaTwoOpponentScore, playerScores[2], opponentScores[2]);
        CompareAndSetTextColor(AreaThreePlayerScore, AreaThreeOpponentScore, playerScores[3], opponentScores[3]);
    }

    private void UpdateRound()
    {
        RoundText.text = RoundManager.Instance.CurrentRound + " / " + RoundManager.Instance.MaxRound;
    }

    private void CompareAndSetTextColor(TMP_Text playerText, TMP_Text opponentText, int playerScore, int opponentScore)
    {
        playerText.color = playerScore > opponentScore ? Color.green : playerScore < opponentScore ? Color.red : Color.yellow;
        opponentText.color = playerScore > opponentScore ? Color.red : playerScore < opponentScore ? Color.green : Color.yellow;
    }

    IEnumerator GiveEffectToScores()
    {
        yield return new WaitForSeconds(0.5f);

        SpawnExplosionEffect(1, AreaOnePlayerScore.rectTransform, AreaOneOpponentScore.rectTransform);
        SpawnExplosionEffect(2, AreaTwoPlayerScore.rectTransform, AreaTwoOpponentScore.rectTransform);
        SpawnExplosionEffect(3, AreaThreePlayerScore.rectTransform, AreaThreeOpponentScore.rectTransform);

        yield return new WaitForSeconds(2f);
    }

    private void SpawnExplosionEffect(int zoneIndex, RectTransform playerScore, RectTransform opponentScore)
    {
        int winner = ScoreManager.Instance.IsPlayerWinningZone(zoneIndex);
        if (winner == 0)
            return;

        Vector3 worldPosition = winner == 1
        ? playerScore.position
        : opponentScore.position;

        EffectManager.Instance.PlayExplosionEffect(worldPosition, 2f);
    }

    public void SetPlayerEnergyText(int playerEnergy)
    {
        PlayerEnergyText.text = playerEnergy.ToString();
    }

    public void ToggleMusicAndUpdate()
    {
        SoundManager.Instance.ToggleMusic();
        UpdateButtonImage();
    }
    private void UpdateButtonImage()
    {
        if (_buttonImage.sprite == MusicOnSprite)
            _buttonImage.sprite = MusicOffSprite;
        else _buttonImage.sprite = MusicOnSprite;
    }

    public void BackToMenuPopup()
    {
        MainMenuPopupPanel.SetActive(true);
        AlterButtons(activate: false);
        RoundManager.Instance.TogglePause();
        SoundManager.Instance.LowerMusicVolume();
    }

    public void HideBackToMenuPopup()
    {
        MainMenuPopupPanel.SetActive(false);
        AlterButtons(activate: true);
        RoundManager.Instance.TogglePause();
        SoundManager.Instance.RestoreOriginalVolume();
    }

    private void AlterButtons(bool activate)
    {
        MainMenuShortcutButton.interactable = activate;
        MusicButton.interactable = activate;
        PlayButton.interactable = activate;
    }
}
