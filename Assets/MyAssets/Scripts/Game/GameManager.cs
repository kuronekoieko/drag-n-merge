using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] AudioManager audioManager;
    [SerializeField] SaveDataManager saveDataManager;
    [SerializeField] TenjinManager tenjinManager;
    [SerializeField] CSVManagr cSVManagr;
    public GameController gameController { get; private set; }
    public UIManager uIManager { get; private set; }
    public static GameManager i;

    void Awake()
    {
        QualitySettings.vSyncCount = 0; // VSyncをOFFにする
        Application.targetFrameRate = 60; // ターゲットフレームレートを60に設定
        Time.fixedDeltaTime = 0.008f;
        gameController = GetComponent<GameController>();
        uIManager = GetComponent<UIManager>();
        i = this;
    }

    void Start()
    {
        saveDataManager.OnStart();
        tenjinManager.OnStart();
        FirebaseAnalyticsManager.i.OnStart();

        cSVManagr.OnStart();
        gameController.OnStart();
        uIManager.OnStart();
        audioManager.OnStart();

        SaveData.i.launchCount += 1;
        saveDataManager.Save();

        Variables.screenState = ScreenState.INITIALIZE;
    }


    void Update()
    {
        switch (Variables.screenState)
        {
            case ScreenState.INITIALIZE:
                Variables.screenState = ScreenState.START;
                gameController.OnInitialize();
                uIManager.OnInitialize();
                FirebaseAnalyticsManager.i.LogScreen("ゲーム");
                FirebaseAnalyticsManager.i.LogEvent("画面_ゲーム");
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
