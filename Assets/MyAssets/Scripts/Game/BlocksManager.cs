﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 【C#】2次元配列の宣言・初期化・代入
/// https://algorithm.joho.info/programming/csharp/2d-array-cs/
/// </summary>
public class BlocksManager : MonoBehaviour
{
    [SerializeField] BlockController blockControllerPrefab;
    BlockController[] blockControllers;
    Dictionary<int, bool> isExistNum = new Dictionary<int, bool>();
    public static BlocksManager i;
    int[,] lines;

    public void OnStart()
    {
        i = this;
        Variables.blockHeight = blockControllerPrefab.GetComponent<BoxCollider2D>().size.x;
        BlockGenerator();
        for (int i = 0; i < Values.TARGET_BLOCK_NUM; i++)
        {
            isExistNum[i + 1] = false;
        }
    }

    public void OnInitialize()
    {
        for (int i = 0; i < blockControllers.Length; i++)
        {
            blockControllers[i].gameObject.SetActive(false);
        }

        //コンボ用
        lines = new int[,]{
            {8,0,1,1,0,8,},
            {7,0,0,0,0,7,},
            {6,0,0,0,0,6,},
            {5,0,0,0,0,5,},
            {4,0,0,0,0,4,},
            {3,0,0,0,0,3,},
            {2,0,0,0,0,2,},
            {1,0,0,0,0,1,},
        };

        lines = new int[,]{
            {7,5,6,8,6,9,},
            {4,3,4,0,5,7,},
            {2,0,0,0,4,2,},
            {0,0,0,0,0,0,},
            {0,0,0,0,0,0,},
            {0,0,0,0,0,0,},
            {0,0,0,0,0,0,},
            {0,0,0,0,0,0,},
        };

        for (int iy = 0; iy < lines.GetLength(0); iy++)
        {
            SetBlocksNewLine(iy + 1);
        }

        // SetBlocksNewLine(1);
        // SetBlocksNewLine(2);
    }

    public BlockController GetBlock(int indexX, int indexY)
    {
        for (int i = 0; i < blockControllers.Length; i++)
        {
            if (blockControllers[i].indexX != indexX) { continue; }
            if (blockControllers[i].indexY != indexY) { continue; }
            if (!blockControllers[i].gameObject.activeSelf) { continue; }
            // if (blockControllers[i].blockState != BlockState.STOP) { continue; }
            return blockControllers[i];
        }
        return null;
    }


    public void OnUpdate()
    {
        for (int i = 0; i < blockControllers.Length; i++)
        {
            blockControllers[i].OnUpdate();
        }
    }

    void BlockGenerator()
    {
        int blockNum = Values.BOARD_LENGTH_X * Values.BOARD_LENGTH_Y;
        blockControllers = new BlockController[blockNum];
        for (int i = 0; i < blockControllers.Length; i++)
        {
            blockControllers[i] = Instantiate(blockControllerPrefab, Vector2.zero, Quaternion.identity, transform);
            blockControllers[i].OnStart();
            blockControllers[i].name = "block" + i;
        }
    }

    public void SetBlocksNewLine(int indexY)
    {
        int ix = 0;

        for (int i = 0; i < blockControllers.Length; i++)
        {
            BlockController block = blockControllers[i];
            if (block.gameObject.activeSelf) { continue; }
            int num = lines[indexY - 1, ix];
            if (num == 0)
            {
                ix++;
                if (ix == Values.BOARD_LENGTH_X) { break; }
                continue;
            }
            block.TransrateBlock(indexX: ix, indexY: indexY);
            block.gameObject.SetActive(true);
            //block.SetNewLine();
            Debug.Log(indexY + ":" + ix);

            if (num != 0) block.SetNewBlock(num);
            ix++;
            if (ix == Values.BOARD_LENGTH_X) { break; }
        }

        //MoveUpAllBlocks();
    }

    public void MoveUpAllBlocks()
    {
        for (int i = 0; i < blockControllers.Length; i++)
        {
            BlockController block = blockControllers[i];
            //block.TransrateBlock(block.indexX, block.indexY + 1);
            block.MoveUpAnim();
        }
    }

    public bool IsDuplicateBlockNum()
    {

        for (int i = 0; i < Values.TARGET_BLOCK_NUM; i++)
        {
            isExistNum[i + 1] = false;
        }

        for (int i = 0; i < blockControllers.Length; i++)
        {
            BlockController block = blockControllers[i];
            if (!block.gameObject.activeSelf) { continue; }
            if (isExistNum[block.num]) { return true; }
            isExistNum[block.num] = true;
        }
        return false;
    }

    public bool IsAllBlockStopped()
    {
        for (int i = 0; i < blockControllers.Length; i++)
        {
            BlockController block = blockControllers[i];
            if (!block.gameObject.activeSelf) { continue; }
            if (block.blockState == BlockState.STOP) { continue; }
            return false;
        }
        return true;

    }
}


public class Line
{
    public int a;
    public int b;
    public int c;
    public int d;
    public int e;
    public int f;
}
