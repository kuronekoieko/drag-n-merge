using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 【C#】2次元配列の宣言・初期化・代入
/// https://algorithm.joho.info/programming/csharp/2d-array-cs/
/// </summary>
public class BlocksManager : MonoBehaviour
{
    [SerializeField] BlockController blockControllerPrefab;
    BlockController[] blockControllers;

    public static BlocksManager i;

    public void OnStart()
    {
        i = this;
        Variables.blockHeight = blockControllerPrefab.GetComponent<BoxCollider2D>().size.x;
        BlockGenerator();

        for (int i = 0; i < 2; i++)
        {
            SetBlocksNewLine();
        }

    }


    public bool IsBlockExist(int indexX, int indexY, out BlockController block)
    {
        for (int i = 0; i < blockControllers.Length; i++)
        {
            if (blockControllers[i].indexX != indexX) { continue; }
            if (blockControllers[i].indexY != indexY) { continue; }
            if (!blockControllers[i].gameObject.activeSelf) { continue; }
            block = blockControllers[i];
            return true;
        }
        block = null;
        return false;
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
        }
    }

    public void SetBlocksNewLine()
    {
        int count = 0;

        for (int i = 0; i < blockControllers.Length; i++)
        {
            BlockController block = blockControllers[i];
            if (block.gameObject.activeSelf) { continue; }

            block.TransrateBlock(indexX: count, indexY: 0);
            block.gameObject.SetActive(true);
            block.SetNewLine();
            count++;
            if (count == Values.BOARD_LENGTH_X) { break; }
        }

        MoveUpAllBlocks();
    }

    void MoveUpAllBlocks()
    {
        for (int i = 0; i < blockControllers.Length; i++)
        {
            BlockController block = blockControllers[i];
            block.TransrateBlock(block.indexX, block.indexY + 1);
        }
    }

    BlockController GetLowestBlock(int indexX)
    {
        int indexY = 100;
        BlockController lowestBlock = null;
        for (int i = 0; i < blockControllers.Length; i++)
        {
            BlockController block = blockControllers[i];
            if (block.indexX != indexX) { continue; }
            if (!block.gameObject.activeSelf) { continue; }
            if (block.indexY >= indexY) { continue; }
            indexY = block.indexY;
            lowestBlock = block;
        }


        return lowestBlock;
    }
}
