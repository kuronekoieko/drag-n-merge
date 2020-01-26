using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

/// <summary>
/// 【C#】2次元配列の宣言・初期化・代入
/// https://algorithm.joho.info/programming/csharp/2d-array-cs/
/// 【C#,LINQ】Select,SelectMany～配列やリスト内の要素の形を変形したいとき～
/// https://www.urablog.xyz/entry/2018/05/28/070000
/// </summary>
public class BlocksManager : MonoBehaviour
{
    [SerializeField] BlockController blockControllerPrefab;
    BlockController[] blockControllers;
    Dictionary<int, bool> isExistNum = new Dictionary<int, bool>();
    public static BlocksManager i;

    void Awake()
    {
        i = this;
    }

    public void OnStart()
    {
        BlockGenerator();
        for (int i = 0; i < Values.TARGET_BLOCK_NUM + 2; i++)
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
        SetBlocksNewLine(1);
        SetBlocksNewLine(2);
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
            string name = "block_" + i;
            blockControllers[i].OnStart(name);
        }
    }

    public void SetBlocksNewLine(int indexY)
    {
        int count = 0;

        for (int i = 0; i < blockControllers.Length; i++)
        {
            BlockController block = blockControllers[i];
            if (block.gameObject.activeSelf) { continue; }

            block.TransrateBlock(indexX: count, indexY: indexY);
            block.gameObject.SetActive(true);
            block.SetNewLine();
            block.SetRandomNum();
            count++;
            if (count == Values.BOARD_LENGTH_X) { break; }
        }
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

        for (int i = 0; i < isExistNum.Count; i++)
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
        return blockControllers
            .Where(b => b.gameObject.activeSelf)
            .All(b => b.blockState == BlockState.STOP);
    }

    public void ShowBlocksTopLine()
    {
        int count = 0;

        for (int i = 0; i < blockControllers.Length; i++)
        {
            BlockController block = blockControllers[i];
            if (block.gameObject.activeSelf) { continue; }
            if (block.blockState == BlockState.DRAG) { continue; }
            block.TransrateBlock(indexX: count, indexY: Values.BOARD_LENGTH_Y - 1);
            block.gameObject.SetActive(true);
            block.SetNewLine();
            block.SetSameNumAsUnderBlock();
            count++;
            if (count == Values.BOARD_LENGTH_X) { break; }
        }
    }

    public void MakeNumbersConsecutive(int indexX)
    {
        BlockController[] blocks = GetBlocksArrayColumn(indexX);
        int num = 0;
        for (int i = blocks.Length - 1; i > -1; i--)
        {
            BlockController block = blocks[i];
            if (block == null) { continue; }
            block.num = num;//バリデーションで0が1になる
            num++;
        }
    }

    BlockController[] GetBlocksArrayColumn(int indexX)
    {
        BlockController[] blocks = new BlockController[Values.BOARD_LENGTH_Y];

        for (int i = 0; i < blockControllers.Length; i++)
        {
            BlockController block = blockControllers[i];
            if (block.indexX != indexX) { continue; }
            if (!block.gameObject.activeSelf) { continue; }
            blocks[block.indexY] = block;
        }




        return blocks;
    }


    public void ShuffleBlocks()
    {
        List<int> numbers = blockControllers
                .Where(block => block.gameObject.activeSelf)
                .Select(block => block.num)
                .ToList();

        List<int> shuffleNumbers = numbers
            .OrderBy(i => Guid.NewGuid())
            .ToList();

        int index = 0;
        for (int i = 0; i < blockControllers.Length; i++)
        {
            BlockController block = blockControllers[i];
            if (block.gameObject.activeSelf == false) { continue; }
            block.num = shuffleNumbers[index];
            index++;
        }
    }

    public void AutoMerge()
    {

    }
}
