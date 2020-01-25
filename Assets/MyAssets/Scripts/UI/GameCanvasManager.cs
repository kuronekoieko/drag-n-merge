using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;

/// <summary>
/// uGUIでボタンの邪魔をするタッチ判定を消すスクリプト
/// http://kohki.hatenablog.jp/entry/uGUI-IgnoreTouch
/// </summary>
public class GameCanvasManager : MonoBehaviour
{
    [SerializeField] Text timerText;
    [SerializeField] Text scoreText;
    [SerializeField] Text bestTargetBlockCountText;
    [SerializeField] Text bestScoreText;
    [SerializeField] Button gameEndButton;
    [SerializeField] Text erasedBlockNumText;
    [SerializeField] Text comboCountText;

    public void OnStart()
    {
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
            .Subscribe(comboCount => { ShowComboCount(); })
            .AddTo(this.gameObject);

        gameEndButton.onClick.AddListener(OnClickGameEndButton);
    }

    public void OnInitialize()
    {
        bestTargetBlockCountText.text = "x " + SaveData.i.eraseTargetBlockCount;
        bestScoreText.text = SaveData.i.sumOfErasedBlockNumbers.ToString();
        comboCountText.gameObject.SetActive(false);
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
        if (Variables.comboCount < 2) { return; }
        comboCountText.gameObject.SetActive(Variables.comboCount > 1);
        comboCountText.text = "Combo x " + Variables.comboCount;
        if (Variables.comboCount != 2) { return; }
        comboCountText.rectTransform.localScale = Vector3.zero;
        comboCountText.rectTransform.DOScale(Vector3.one, 0.5f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {

            });
    }

}
