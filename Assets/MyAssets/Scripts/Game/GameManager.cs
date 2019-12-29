﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameCanvasManager gameCanvasManager;
    GameController gameController;
    void Awake()
    {
        QualitySettings.vSyncCount = 0; // VSyncをOFFにする
        Application.targetFrameRate = 60; // ターゲットフレームレートを60に設定
    }

    void Start()
    {
        gameController = GetComponent<GameController>();
        gameController.OnStart();
        gameCanvasManager.OnStart();
        Variables.resultState = ResultState.PLAYING;
    }


    void Update()
    {
        gameController.OnUpdate();
    }
}
