using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class ResultCanvasManager : MonoBehaviour
{
    [SerializeField] Text resultText;
    public void OnStart()
    {
        /*
         this.ObserveEveryValueChanged(timer => Variables.timer)
            .Subscribe(timer => { timerText.text = timer.ToString("F2"); })
            .AddTo(this.gameObject);
        */

    }
}
