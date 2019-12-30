﻿using System.Collections;
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
            .Subscribe(timer => { timerText.text = timer.ToString("F2"); })
            .AddTo(this.gameObject);

        targetNumText.text = Variables.targetNum.ToString();
    }


}
