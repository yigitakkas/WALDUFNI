using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Battleground : MonoBehaviour
{
    public SpriteRenderer BgImage;
    private bool _activated=false;
    private BattlegroundEffect _battlegroundEffect;
    public TMP_Text Description;
    public TMP_Text Name;

    private void Awake()
    {

    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void ActivateBattleground(BattlegroundEffect battlegroundEffect, Sprite sprite, string description, string name)
    {
        BgImage.enabled = true;
        BgImage.sprite = sprite;
        _battlegroundEffect = battlegroundEffect;
        _activated = true;

        Name.text = name;
        Name.gameObject.SetActive(true);
        Description.text = description;
        Description.gameObject.SetActive(true);
    }
}
