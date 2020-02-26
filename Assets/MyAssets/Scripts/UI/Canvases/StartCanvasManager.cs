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
    public readonly ScreenState thisScreen = ScreenState.START;

    public override void OnStart()
    {
        startButton.onClick.AddListener(OnClickStartButton);

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
        gameObject.SetActive(true);
        GameManager.i.gameController.backgroundController.SetBackground();
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
