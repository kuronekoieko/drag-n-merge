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

    [SerializeField] SpriteRenderer bGSpriteRenderer;

    public override void OnStart()
    {
        base.screenState = ScreenState.START;
        startButton.onClick.AddListener(OnClickStartButton);

        // base.SetScreenAction(thisScreen: ScreenState.START);

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

    public override void Open()
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

    public override void Close()
    {
    }

    void OnClickStartButton()
    {
        FirebaseAnalyticsManager.i.LogEvent("スタートボタン");
        gameObject.SetActive(false);
        // Variables.screenState = ScreenState.GAME;
        GameManager.i.uIManager.Change(open: ScreenState.GAME, close: base.screenState);

        AudioManager.i.PlayOneShot(1);
    }
}
