using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;

/// <summary>
/// 【Unity】[TIPS] DoTweenで画像のalpha値をアニメーションさせたい
/// https://ghoul-life.hatenablog.com/entry/2018/02/01/201750
/// </summary>
public class ContinueCanvasManager : BaseCanvasManager
{
    [SerializeField] Button coinButton;
    [SerializeField] Button videoButton;
    [SerializeField] Button cancelButton;
    [SerializeField] Text countText;
    [SerializeField] Text coinCountText;
    [SerializeField] Image circleImage;
    [SerializeField] Text cancelText;

    float timer;
    float startTime = 9;
    int coinCount = 200;

    public override void OnStart()
    {
        base.screenState = ScreenState.CONTINUE;
        //base.SetScreenAction(thisScreen: ScreenState.CONTINUE);

        coinButton.onClick.AddListener(OnClickCoinButton);
        videoButton.onClick.AddListener(OnClickVideoButton);
        cancelButton.onClick.AddListener(OnClickCancelButton);
        coinCountText.text = coinCount.ToString();
    }

    public override void OnInitialize()
    {
        gameObject.SetActive(false);
        timer = startTime;
    }

    void Update()
    {
        if (Variables.screenState != ScreenState.CONTINUE) { return; }

        if (timer < 0)
        {
            Variables.screenState = ScreenState.RESULT;
            return;
        }
        timer -= Time.deltaTime;
        countText.text = Mathf.CeilToInt(timer).ToString();
        circleImage.fillAmount = timer / startTime;
    }

    public override void Open()
    {
        gameObject.SetActive(true);
        cancelButton.gameObject.SetActive(false);
        DOVirtual.DelayedCall(2, () =>
        {
            cancelButton.gameObject.SetActive(true);

            Color c = cancelText.color;
            c.a = 0;
            cancelText.color = c;

            DOTween.ToAlpha(
                () => cancelText.color,
                color => cancelText.color = color,
                1f, // 目標値
                0.5f // 所要時間
                );
        });
    }

    public override void Close()
    {
        gameObject.SetActive(false);
    }

    void OnClickCoinButton()
    {
        if (SaveData.i.coinCount < coinCount) { return; }
        SaveData.i.coinCount -= coinCount;
        SaveDataManager.i.Save();
        Continue();
        AudioManager.i.PlayOneShot(3);
    }

    void OnClickVideoButton()
    {
        Continue();
    }

    void OnClickCancelButton()
    {
        //Variables.screenState = ScreenState.RESULT;
        GameManager.i.uIManager.Change(open: ScreenState.RESULT, close: base.screenState);
    }

    void Continue()
    {
        for (int i = 1; i < 6; i++)
        {
            BlocksManager.i.EraseLine(i);
        }

        // Variables.screenState = ScreenState.GAME;
        GameManager.i.uIManager.Change(open: ScreenState.GAME, close: base.screenState);
    }

}
