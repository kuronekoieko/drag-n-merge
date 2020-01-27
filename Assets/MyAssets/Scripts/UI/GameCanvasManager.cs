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
    [SerializeField] Sprite[] itemSprites;
    Sequence sequence;
    Color defaultColor;
    ItemButtonController[] itemButtons;
    UnityAction[] itemButtonUAs;

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

        itemButtonUAs = new UnityAction[4];
        itemButtonUAs[0] = OnClickAddTimeButton;
        itemButtonUAs[1] = OnClickFallBlockButton;
        itemButtonUAs[2] = OnClickShuffleButton;
        itemButtonUAs[3] = OnClickAutoMergeButton;

        itemButtons = new ItemButtonController[4];

        Vector3 pos = new Vector3(-140, 130, 0);
        for (int i = 0; i < itemButtons.Length; i++)
        {
            itemButtons[i] = Instantiate(itemButtonPrefab, Vector3.zero, Quaternion.identity, itemButtonsParent);
            itemButtons[i].OnStart(itemSprites[i], pos, itemButtonUAs[i]);
            pos.x += 140;
        }

        gameEndButton.onClick.AddListener(OnClickGameEndButton);
        defaultColor = comboCountText.color;
    }

    public override void OnInitialize()
    {
        bestTargetBlockCountText.text = "x " + SaveData.i.eraseTargetBlockCount;
        bestScoreText.text = SaveData.i.sumOfErasedBlockNumbers.ToString();
        comboCountText.gameObject.SetActive(false);
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
        FirebaseAnalyticsManager.i.LogEvent("ゲーム終了ボタン");
        Variables.screenState = ScreenState.RESULT;
    }


    void ShowComboCount()
    {
        sequence.Kill();
        sequence = DOTween.Sequence()
            .OnStart(() =>
            {
                comboCountText.gameObject.SetActive(true);
                comboCountText.text = "Combo\nx " + Variables.comboCount;
                comboCountText.rectTransform.localScale = Vector3.zero;
                comboCountText.color = defaultColor;
            })
            .Append(comboCountText.rectTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack))
            .Append(DOTween.ToAlpha(() => comboCountText.color, color => comboCountText.color = color, 0f, 2f));
    }

    void OnClickAddTimeButton()
    {
        if (!BlocksManager.i.IsAllBlockStopped()) { return; }
        Variables.timer += 5;
    }

    void OnClickFallBlockButton()
    {
        if (!BlocksManager.i.IsAllBlockStopped()) { return; }
        BlocksManager.i.ShowBlocksTopLine();
    }



    void OnClickShuffleButton()
    {
        if (!BlocksManager.i.IsAllBlockStopped()) { return; }
        BlocksManager.i.ShuffleBlocks();
    }

    void OnClickAutoMergeButton()
    {
        if (!BlocksManager.i.IsAllBlockStopped()) { return; }
        BlocksManager.i.AutoMergeBlocks();
    }

}
