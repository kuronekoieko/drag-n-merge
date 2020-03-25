using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Variables : MonoBehaviour
{
    public static ScreenState screenState
    {
        set
        {
            lastScreenState = _screenState;
            _screenState = value;
        }
        get { return _screenState; }
    }
    private static ScreenState _screenState;
    public static ScreenState lastScreenState { get; private set; }
    public static GameState gameState;
    public static float timer;
    public static int targetNum;
    public static bool isDragging;
    public static int eraseTargetBlockCount;
    public static int sumOfErasedBlockNumbers;
    public static int comboCount;
    public static List<MasterData> masterDatas;
    public static int autoMergePoint;
    public static List<int[,]> stageNums;
    public static int adStageIndex;
}
