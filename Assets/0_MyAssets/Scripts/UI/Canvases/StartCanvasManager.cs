using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using System.Linq;

public class StartCanvasManager : BaseCanvasManager
{
    [SerializeField] Button startButton;
    [SerializeField] Text startText;
    [SerializeField] Button skinButton;

    public readonly ScreenState thisScreen = ScreenState.START;

    public override void OnStart()
    {
        startButton.onClick.AddListener(OnClickStartButton);
        skinButton.onClick.AddListener(OnClickSkinButton);

        base.SetScreenAction(thisScreen: thisScreen);

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
        FirebaseAnalyticsManager.i.LogEvent("画面_スタート");
        gameObject.SetActive(true);
        GameManager.i.gameController.backgroundController.SetBackground();
    }

    protected override void OnClose()
    {
    }

    void OnClickStartButton()
    {
        gameObject.SetActive(false);
        Variables.screenState = ScreenState.GAME;
        AudioManager.i.PlayOneShot(1);
        Variables.screenState = ScreenState.REWARD;
    }

    void OnClickSkinButton()
    {
        Variables.screenState = ScreenState.SKIN;
        AudioManager.i.PlayOneShot(1);
        FirebaseAnalyticsManager.i.LogEvent("スタート_ショップボタン");
    }
}
