using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance;

    public static event Action OnRoundStarted;
    public static event Action OnRoundEnded;
    public static event Action GameEnded;

    public int CurrentRound { get; private set; }

    [SerializeField]
    private List<PlayArea> _playerPlayAreas = new List<PlayArea>();
    public List<PlayArea> PlayerPlayAreas
    {
        get => _playerPlayAreas; 
        private set => _playerPlayAreas = value; 
    }


    [SerializeField]
    private List<PlayArea> _opponentPlayAreas = new List<PlayArea>();
    public List<PlayArea> OpponentPlayAreas
    {
        get => _opponentPlayAreas;
        private set => _opponentPlayAreas = value;
    }

    public UnityEngine.UI.Button Round4;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        SetRound(1);
        FindAreas();
    }

    private void Start()
    {
        _opponentPlayAreas = OpponentManager.Instance.ReturnOpponentAreas();
        ScoreManager.Instance.CalculatePower(_playerPlayAreas, _opponentPlayAreas);
    }
    public void SetRound(int round)
    {
        CurrentRound = round;
    }
    private void FindAreas()
    {
        foreach (PlayArea area in GetComponentsInChildren<PlayArea>())
        {
            if (area.CompareTag("PlayerArea"))
            {
                _playerPlayAreas.Add(area);
            }
        }
    }
    public void StartRound()
    {
        StartCoroutine(HandleRoundFlow());
    }
    private IEnumerator HandleRoundFlow()
    {
        if (CurrentRound < 6)
        {
            OnRoundStarted?.Invoke();
            yield return new WaitForSeconds(0.5f);//Opponent kart ekleme animasyon süresi

            ApplyCardEffects();
            yield return new WaitForSeconds(0.5f); //Önce kart efektleri, sonra battleground efektleri

            BattlegroundManager.Instance.ApplyBattlegroundEffects();
            yield return null;

            ScoreManager.Instance.CalculatePower(_playerPlayAreas, _opponentPlayAreas);
            yield return new WaitForSeconds(0.5f);

            UIManager.Instance.UpdateUI();
            if (CurrentRound == 5)
            {
                GameEnded?.Invoke();
                yield break;
            }
            CurrentRound++;
            yield return new WaitForSeconds(1f);
            OnRoundEnded?.Invoke();
        }
    }


    public PlayArea GetAreaWithIndex(int index, bool player)
    {
        if(player)
        {
            return PlayerPlayAreas[index-1];
        }
        else
        {
            return OpponentPlayAreas[index-1];
        }
    }

    private void PlayAreaCardEffects()
    {
        foreach (PlayArea area in PlayerPlayAreas)
        {
            if (area.PlacedAmount() > 0)
            {
                area.CheckCardEffects();
            }
        }
        foreach (PlayArea area in OpponentPlayAreas)
        {
            if (area.PlacedAmount() > 0)
            {
                area.CheckCardEffects();
            }
        }
    }

    public void ApplyCardEffects()
    {
        foreach(PlayArea playerPlayArea in _playerPlayAreas)
        {
            foreach(Card card in playerPlayArea.PlacedCardsThisRound)
            {
                if (card.CardTriggerType == CardTrigger.OnReveal)
                {
                    card.TriggerCardEffect();
                }
            }
            foreach(Card card in playerPlayArea.PlacedCards)
            {
                if(card.CardTriggerType == CardTrigger.Ongoing)
                {
                    card.TriggerCardEffect();
                }
            }
        }
        foreach (PlayArea opponentPlayArea in _opponentPlayAreas)
        {
            foreach (Card card in opponentPlayArea.PlacedCardsThisRound)
            {
                if (card.CardTriggerType == CardTrigger.OnReveal)
                {
                    card.TriggerCardEffect();
                }
            }
            foreach (Card card in opponentPlayArea.PlacedCards)
            {
                if (card.CardTriggerType == CardTrigger.Ongoing)
                {
                    card.TriggerCardEffect();
                }
            }
        }
        PlayAreaCardEffects();
    }

    public void MainMenu()
    {
        //Main Menuyu yükle
        SceneManager.LoadScene(0);
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(1);
    }

    public void MakeRound4()
    {
        CurrentRound = 4;
    }
}