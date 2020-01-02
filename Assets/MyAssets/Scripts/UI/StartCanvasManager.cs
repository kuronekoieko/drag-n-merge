﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;

public class StartCanvasManager : MonoBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] Text targetNumText;
    [SerializeField] Image targetBlockImage;
    [SerializeField] Text startText;
    // [SerializeField] Text levelText;

    public void OnStart()
    {
        startButton.onClick.AddListener(OnClickStartButton);
        gameObject.SetActive(true);


        this.ObserveEveryValueChanged(screenState => Variables.screenState)
           .Where(screenState => screenState == ScreenState.START)
           .Subscribe(timer => { OnOpen(); })
           .AddTo(this.gameObject);

        Anim();
    }

    public void OnInitialize()
    {
        targetNumText.text = Values.TARGET_BLOCK_NUM.ToString();
        targetNumText.color = BlockColorData.i.blockColors[Values.TARGET_BLOCK_NUM - 1].textColor;
        targetBlockImage.color = BlockColorData.i.blockColors[Values.TARGET_BLOCK_NUM - 1].color;
        // levelText.text = "LEVEL  " + (SaveData.i.clearedLevel + 1);
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

    void OnOpen()
    {
        gameObject.SetActive(true);
    }

    void OnClickStartButton()
    {
        gameObject.SetActive(false);
        Variables.screenState = ScreenState.GAME;
        AudioManager.i.PlayOneShot(1);
    }
}
