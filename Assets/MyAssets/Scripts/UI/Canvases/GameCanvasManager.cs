using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using UnityEngine.Events;

/// <summary>
/// uGUIでボタンの邪魔をするタッチ判定を消すスクリプト
/// http://kohki.hatenablog.jp/entry/uGUI-IgnoreTouch
/// [Unity][DoTween]DoTweenで調べまくって辿りついた いくつかの事
/// https://qiita.com/3panda/items/0a8c93645087b6b6d728
/// </summary>
public class GameCanvasManager : BaseCanvasManager
{
    [SerializeField] Text timerText;
    [SerializeField] Text scoreText;
    [SerializeField] Text bestTargetBlockCountText;
    [SerializeField] Text bestScoreText;
    [SerializeField] Button gameEndButton;
    [SerializeField] Text erasedBlockNumText;
    [SerializeField] Text comboCountText;
    [SerializeField] Transform itemButtonsParent;
    [SerializeField] ItemButtonController itemButtonPrefab;
    [SerializeField] Button autoMergeButton;
    [SerializeField] Text autoMergeCountText;
    [SerializeField] Image autoMergeCircleImage;
    [SerializeField] Image autoMergeBadgeImage;
    [SerializeField] Text addTimeText;

    Sequence sequence;
    Sequence addTimeTextSequence;
    Color defaultColor;
    ItemButtonController[] itemButtons;
    int autoMergeCount;

    public override void OnStart()
    {
        base.SetScreenAction(thisScreen: ScreenState.GAME);

        this.ObserveEveryValueChanged(timer => Variables.timer)
            .Subscribe(timer => { SetTimeCountText(); })
            .AddTo(this.gameObject);

        this.ObserveEveryValueChanged(count => Variables.eraseTargetBlockCount)
            .Subscribe(count => { scoreText.text = "x " + count; })
            .AddTo(this.gameObject);

        this.ObserveEveryValueChanged(num => Variables.sumOfErasedBlockNumbers)
            .Subscribe(num => { erasedBlockNumText.text = "" + num; })
            .AddTo(this.gameObject);

        this.ObserveEveryValueChanged(comboCount => Variables.comboCount)
            .Where(comboCount => comboCount > 1)
            .Subscribe(comboCount => { ShowComboCount(); })
            .AddTo(this.gameObject);

        this.ObserveEveryValueChanged(autoMergeCount => this.autoMergeCount)
            .Subscribe(autoMergeCount => { autoMergeCountView(); })
            .AddTo(this.gameObject);

        this.ObserveEveryValueChanged(autoMergePoint => Variables.autoMergePoint)
            .Subscribe(autoMergePoint => { AutoMergeCircleView(); })
            .AddTo(this.gameObject);

        ItemButtonGenerator();

        autoMergeButton.onClick.AddListener(OnClickAutoMergeButton);

        gameEndButton.onClick.AddListener(OnClickGameEndButton);
        defaultColor = comboCountText.color;
    }

    public override void OnInitialize()
    {
        bestTargetBlockCountText.text = "x " + SaveData.i.eraseTargetBlockCount;
        bestScoreText.text = SaveData.i.sumOfErasedBlockNumbers.ToString();
        comboCountText.gameObject.SetActive(false);
        Variables.autoMergePoint = 0;
        autoMergeCount = 0;
    }

    protected override void OnOpen()
    {
    }

    protected override void OnClose()
    {
    }

    void SetTimeCountText()
    {
        string timer = Variables.timer.ToString("F2");
        if (Variables.timer < 0) timer = "0.00";
        timerText.text = timer;
    }

    void OnClickGameEndButton()
    {
        FirebaseAnalyticsManager.i.LogEvent("ポーズボタン");
        Variables.screenState = ScreenState.PAUSE;
    }


    void ShowComboCount()
    {
        sequence.Kill();
        sequence = DOTween.Sequence()
            .OnStart(() =>
            {
                comboCountText.gameObject.SetActive(true);
                comboCountText.text = "COMBO\nx " + Variables.comboCount;
                comboCountText.rectTransform.localScale = Vector3.zero;
                comboCountText.color = defaultColor;
            })
            .Append(comboCountText.rectTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack))
            .Append(DOTween.ToAlpha(() => comboCountText.color, color => comboCountText.color = color, 0f, 2f));
    }



    void ItemButtonGenerator()
    {

        var buttonActions = new List<UnityAction>()
        {
            OnClickAddTimeButton,
            OnClickFallBlockButton,
            OnClickShuffleButton,
            OnClickAutoMergeButton,
        };

        itemButtons = new ItemButtonController[SaveData.i.itemCounts.Length];

        Vector3 pos = new Vector3(0, 130, 0);
        for (int i = 0; i < itemButtons.Length; i++)
        {
            itemButtons[i] = Instantiate(itemButtonPrefab, Vector3.zero, Quaternion.identity, itemButtonsParent);
            itemButtons[i].OnStart(index: i, pos: pos, buttonActions[i]);
            pos.x += 140;
        }
    }


    void OnClickAddTimeButton()
    {
        AudioManager.i.PlayOneShot(3);

        float addSec = 10;
        Variables.timer += addSec;

        addTimeText.text = "+" + addSec;

        addTimeTextSequence.Kill();
        addTimeTextSequence = DOTween.Sequence()
                .OnStart(() =>
                {
                    addTimeText.gameObject.SetActive(true);
                    addTimeText.rectTransform.localScale = Vector3.zero;
                    Color color = addTimeText.color;
                    color.a = 1;
                    addTimeText.color = color;
                })
                .Append(addTimeText.rectTransform.DOScale(Vector3.one, 1f).SetEase(Ease.OutElastic))
                .Append(DOTween.ToAlpha(() => addTimeText.color, color => addTimeText.color = color, 0f, 2f));
    }

    void OnClickFallBlockButton()
    {
        AudioManager.i.PlayOneShot(3);

        BlocksManager.i.ShowBlocksTopLine();
    }

    void OnClickShuffleButton()
    {
        AudioManager.i.PlayOneShot(3);

        BlocksManager.i.ShuffleBlocks();
    }

    void OnClickAutoMergeButton()
    {
        if (autoMergeCount < 1) { return; }
        //無限オートマージ対策
        if (Variables.gameState != GameState.IN_PROGRESS_TIMER) { return; }

        autoMergeCount--;
        BlocksManager.i.AutoMergeBlocks();
        AudioManager.i.PlayOneShot(3);
    }

    void AutoMergeCircleView()
    {
        autoMergeCircleImage.fillAmount = Variables.autoMergePoint / 1500f;
        if (autoMergeCircleImage.fillAmount < 1) { return; }
        autoMergeCount++;
        Variables.autoMergePoint = 0;
    }


    void autoMergeCountView()
    {
        autoMergeBadgeImage.gameObject.SetActive(autoMergeCount != 0);
        autoMergeCountText.text = autoMergeCount.ToString();
    }
}
