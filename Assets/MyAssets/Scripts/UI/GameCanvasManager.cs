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

    public void OnStart()
    {
        this.ObserveEveryValueChanged(timer => Variables.timer)
            .Subscribe(timer => { SetTimeCountText(); })
            .AddTo(this.gameObject);
    }

    public void OnInitialize()
    {
        targetNumText.text = Variables.targetNum.ToString();
        targetNumText.color = BlockColorData.i.blockColors[Variables.targetNum - 1].textColor;
        targetBlockImage.color = BlockColorData.i.blockColors[Variables.targetNum - 1].color;
    }

    void SetTimeCountText()
    {
        string timer = Variables.timer.ToString("F2");
        if (Variables.timer < 0) timer = "0.00";
        timerText.text = timer;
    }

}
