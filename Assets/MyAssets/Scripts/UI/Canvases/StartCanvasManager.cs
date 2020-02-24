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
    [SerializeField] Button shopButton;
    [SerializeField] SpriteRenderer bGSpriteRenderer;

    public override void OnStart()
    {
        startButton.onClick.AddListener(OnClickStartButton);
        shopButton.onClick.AddListener(() =>
        {
            Variables.screenState = ScreenState.SKIN;
        });
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

        for (int i = 0; i < SaveData.i.possessedBackgrounds.Count; i++)
        {
            if (SaveData.i.possessedBackgrounds[i].isSelected)
            {
                bGSpriteRenderer.sprite = BackgroundDataSO.i.backgroundDatas[i].sprite;
            }
        }
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
