using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Utils : MonoBehaviour
{
    public static int PositionToIndexX(float pos_x)
    {
        double ret = Math.Round((pos_x - Variables.blockLowerLeftPos.x) / Values.BROCK_DISTANCE, 0);
        return (int)ret;
    }

    public static int PositionToIndexY(float pos_y)
    {
        double ret = Math.Round((pos_y - Variables.blockLowerLeftPos.y) / Values.BROCK_DISTANCE, 0);
        int indexY = (int)ret;
        return indexY;
    }

    public static Vector2 IndexToPosition(int index_x, int index_y)
    {
        float x = Variables.blockLowerLeftPos.x + Values.BROCK_DISTANCE * index_x;
        float y = Variables.blockLowerLeftPos.y + Values.BROCK_DISTANCE * index_y;
        return new Vector2(x, y);
    }
}
