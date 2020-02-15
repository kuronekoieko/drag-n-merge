using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Utils : MonoBehaviour
{
    public static int PositionToIndexX(float pos_x)
    {
        double ret = Math.Round((pos_x - Values.BLOCK_POS_LOWER_LEFT.x) / Values.BROCK_DISTANCE, 0);
        return (int)ret;
    }

    public static int PositionToIndexY(float pos_y)
    {
        double ret = Math.Round((pos_y - Values.BLOCK_POS_LOWER_LEFT.y) / Values.BROCK_DISTANCE, 0);
        int indexY = (int)ret;
        return indexY;
    }

    public static Vector2 IndexToPosition(int index_x, int index_y)
    {
        float x = Values.BLOCK_POS_LOWER_LEFT.x + Values.BROCK_DISTANCE * index_x;
        float y = Values.BLOCK_POS_LOWER_LEFT.y + Values.BROCK_DISTANCE * index_y;
        return new Vector2(x, y);
    }

    public static bool IsLanguageJapanese()
    {
        string deviceLanguage = Application.systemLanguage.ToString();
        return deviceLanguage.Equals("Japanese");
    }

    public static MasterData GetMasterData(int targetBlockCount)
    {
        MasterData lastMasterData = Variables.masterDatas.Last();
        int lastIndex = Variables.masterDatas.LastIndexOf(lastMasterData);
        if (lastIndex < targetBlockCount) { return lastMasterData; }
        return Variables.masterDatas[targetBlockCount];
    }
}
