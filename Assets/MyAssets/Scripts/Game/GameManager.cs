using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] StartCanvasManager startCanvasManager;
    [SerializeField] GameCanvasManager gameCanvasManager;
    [SerializeField] ResultCanvasManager resultCanvasManager;
    [SerializeField] AudioManager audioManager;
    GameController gameController;
    void Awake()
    {
        QualitySettings.vSyncCount = 0; // VSyncをOFFにする
        Application.targetFrameRate = 60; // ターゲットフレームレートを60に設定
        Time.fixedDeltaTime = 0.008f;
    }

    void Start()
    {
        gameController = GetComponent<GameController>();
        gameController.OnStart();
        gameCanvasManager.OnStart();
        resultCanvasManager.OnStart();
        startCanvasManager.OnStart();
        audioManager.OnStart();
        Variables.resultState = ResultState.PLAYING;
        Variables.screenState = ScreenState.INITIALIZE;
    }


    void Update()
    {
        switch (Variables.screenState)
        {
            case ScreenState.INITIALIZE:
                Variables.screenState = ScreenState.START;
                gameController.Initialize();
                break;
            case ScreenState.START:
                break;
            case ScreenState.GAME:
                gameController.OnUpdate();
                break;
            case ScreenState.RESULT:
                break;
        }

    }
}
