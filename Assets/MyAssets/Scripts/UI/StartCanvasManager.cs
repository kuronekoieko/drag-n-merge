using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;

public class StartCanvasManager : MonoBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] Text startText;

    public void OnStart()
    {
        startButton.onClick.AddListener(OnClickStartButton);



        this.ObserveEveryValueChanged(screenState => Variables.screenState)
           .Where(screenState => screenState == ScreenState.START)
           .Subscribe(timer => { OnOpen(); })
           .AddTo(this.gameObject);

        Anim();
    }

    public void OnInitialize()
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

    void OnOpen()
    {
        gameObject.SetActive(true);
    }

    void OnClickStartButton()
    {
        FirebaseAnalyticsManager.i.LogEvent("スタートボタン");
        gameObject.SetActive(false);
        Variables.screenState = ScreenState.GAME;
        AudioManager.i.PlayOneShot(1);
    }
}
