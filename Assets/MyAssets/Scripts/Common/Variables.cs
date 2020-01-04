using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Variables : MonoBehaviour
{
    public static ScreenState screenState;
    public static GameState gameState;
    public static float timer;
    public static int targetNum;
    public static Vector2 blockLowerLeftPos = new Vector2(-2.0f, -3.6f);
    public static float blockHeight;
    public static bool isDragging;
    public static int eraseTargetBlockCount;
    public static int sumOfErasedBlockNumbers;
}
