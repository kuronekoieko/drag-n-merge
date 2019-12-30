using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class GameCanvasManager : MonoBehaviour
{
    [SerializeField] Text timerText;
    [SerializeField] Text targetNumText;
    public void OnStart()
    {
        this.ObserveEveryValueChanged(timer => Variables.timer)
            .Subscribe(timer => { SetTimeCountText(); })
            .AddTo(this.gameObject);

        targetNumText.text = Variables.targetNum.ToString();
    }

    void SetTimeCountText()
    {
        string timer = Variables.timer.ToString("F2");
        if (Variables.timer < 0) timer = "0.00";
        timerText.text = timer;
    }

}
