﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;

public class StartCanvasManager : BaseCanvasManager
{
    [SerializeField] Button startButton;
    [SerializeField] Text startText;

    public override void OnStart()
    {
        startButton.onClick.AddListener(OnClickStartButton);

        base.SetScreenAction(thisScreen: ScreenState.START);

        Anim();
    }

    public override void OnInitialize()
    {
        gameObject.SetActive(true);
    }

    void Anim()
    {
        startText.transform.DOScale(1.1f, 0.5f)
               .OnComplete(() =>
               {
                   startText.transform.DOScale(1f, 0.5f)
                           .OnComplete(() =>
                           {
                               Anim();
                           });
               });
    }

    protected override void OnOpen()
    {
        gameObject.SetActive(true);
    }

    protected override void OnClose()
    {
    }

    void OnClickStartButton()
    {
        FirebaseAnalyticsManager.i.LogEvent("スタートボタン");
        gameObject.SetActive(false);
        Variables.screenState = ScreenState.GAME;
        AudioManager.i.PlayOneShot(1);
    }
}