using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class GameCanvasManager : MonoBehaviour
{
    [SerializeField] Text timerText;
    [SerializeField] Text targetNumText;
    [SerializeField] Image targetBlockImage;
    [SerializeField] Text scoreText;
    [SerializeField] Text highScoreNumText;
    [SerializeField] Button gameEndButton;

    public void OnStart()
    {
        this.ObserveEveryValueChanged(timer => Variables.timer)
            .Subscribe(timer => { SetTimeCountText(); })
            .AddTo(this.gameObject);

        this.ObserveEveryValueChanged(count => Variables.eraseTargetBlockCount)
            .Subscribe(count => { scoreText.text = "x " + count; })
            .AddTo(this.gameObject);

        gameEndButton.onClick.AddListener(OnClickGameEndButton);
    }

    public void OnInitialize()
    {
        targetNumText.text = Values.TARGET_BLOCK_NUM.ToString();
        targetNumText.color = BlockColorData.i.blockColors[Values.TARGET_BLOCK_NUM - 1].textColor;
        targetBlockImage.color = BlockColorData.i.blockColors[Values.TARGET_BLOCK_NUM - 1].color;
        highScoreNumText.text = SaveData.i.eraseTargetBlockCount.ToString();
        // levelText.text = "LEVEL  " + (SaveData.i.clearedLevel + 1);
    }

    void SetTimeCountText()
    {
        string timer = Variables.timer.ToString("F2");
        if (Variables.timer < 0) timer = "0.00";
        timerText.text = timer;
    }

    void OnClickGameEndButton()
    {
        Variables.screenState = ScreenState.RESULT;
    }

}
