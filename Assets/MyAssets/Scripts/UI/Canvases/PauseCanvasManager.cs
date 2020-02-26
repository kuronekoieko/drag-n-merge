using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseCanvasManager : BaseCanvasManager
{
    [SerializeField] Button shopButton;
    [SerializeField] Button resultButton;
    [SerializeField] Button closeButton;

    public readonly ScreenState thisScreen = ScreenState.PAUSE;

    public override void OnStart()
    {
        base.SetScreenAction(thisScreen: thisScreen);
        gameObject.SetActive(false);

        shopButton.onClick.AddListener(OnClickSkinButton);
        resultButton.onClick.AddListener(OnClickResultButton);
        closeButton.onClick.AddListener(OnClickCloseButton);
    }

    public override void OnInitialize()
    {
    }

    protected override void OnOpen()
    {
        gameObject.SetActive(true);
    }

    protected override void OnClose()
    {
        gameObject.SetActive(false);
    }

    void OnClickSkinButton()
    {
        Variables.screenState = ScreenState.SKIN;
        AudioManager.i.PlayOneShot(1);
    }

    void OnClickResultButton()
    {
        Variables.screenState = ScreenState.RESULT;
        FirebaseAnalyticsManager.i.LogEvent("ゲーム終了ボタン");
    }

    void OnClickCloseButton()
    {
        AudioManager.i.PlayOneShot(5);
        Variables.screenState = ScreenState.GAME;
    }
}
