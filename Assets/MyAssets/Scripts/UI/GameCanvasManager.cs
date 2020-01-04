﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class GameCanvasManager : MonoBehaviour
{
    [SerializeField] Text timerText;
    [SerializeField] Text scoreText;
    [SerializeField] Text bestTargetBlockCountText;
    [SerializeField] Text bestScoreText;
    [SerializeField] Button gameEndButton;
    [SerializeField] Text erasedBlockNumText;

    public void OnStart()
    {
        this.ObserveEveryValueChanged(timer => Variables.timer)
            .Subscribe(timer => { SetTimeCountText(); })
            .AddTo(this.gameObject);

        this.ObserveEveryValueChanged(count => Variables.eraseTargetBlockCount)
            .Subscribe(count => { scoreText.text = "x " + count; })
            .AddTo(this.gameObject);

        this.ObserveEveryValueChanged(num => Variables.sumOfErasedBlockNumbers)
                   .Subscribe(num => { erasedBlockNumText.text = "" + num; })
                   .AddTo(this.gameObject);

        gameEndButton.onClick.AddListener(OnClickGameEndButton);
    }

    public void OnInitialize()
    {
        bestTargetBlockCountText.text = "x " + SaveData.i.eraseTargetBlockCount;
        bestScoreText.text = SaveData.i.sumOfErasedBlockNumbers.ToString();
    }

    void SetTimeCountText()
    {
        string timer = Variables.timer.ToString("F2");
        if (Variables.timer < 0) timer = "0.00";
        timerText.text = timer;
    }

    void OnClickGameEndButton()
    {
        FirebaseAnalyticsManager.i.LogEvent("ゲーム終了ボタン");
        Variables.screenState = ScreenState.RESULT;
    }

}
