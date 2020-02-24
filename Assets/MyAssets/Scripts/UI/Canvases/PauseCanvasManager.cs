using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseCanvasManager : BaseCanvasManager
{
    [SerializeField] Button shopButton;
    [SerializeField] Button resultButton;
    [SerializeField] Button closeButton;
    public override void OnStart()
    {
        base.screenState = ScreenState.PAUSE;
        // base.SetScreenAction(thisScreen: ScreenState.PAUSE);
        gameObject.SetActive(false);

        shopButton.onClick.AddListener(OnClickSkinButton);
        resultButton.onClick.AddListener(OnClickResultButton);
        closeButton.onClick.AddListener(OnClickCloseButton);
    }

    public override void OnInitialize()
    {
    }

    public override void Open()
    {
        gameObject.SetActive(true);
    }

    public override void Close()
    {
        gameObject.SetActive(false);
    }

    void OnClickSkinButton()
    {
        // Variables.screenState = ScreenState.SKIN;
        GameManager.i.uIManager.Change(open: ScreenState.SKIN);
    }

    void OnClickResultButton()
    {
        // Variables.screenState = ScreenState.RESULT;
        GameManager.i.uIManager.Change(open: ScreenState.RESULT, close: base.screenState);
        FirebaseAnalyticsManager.i.LogEvent("ゲーム終了ボタン");
    }

    void OnClickCloseButton()
    {
        //   Variables.screenState = ScreenState.GAME;
        GameManager.i.uIManager.Change(open: ScreenState.GAME, close: base.screenState);
    }
}
