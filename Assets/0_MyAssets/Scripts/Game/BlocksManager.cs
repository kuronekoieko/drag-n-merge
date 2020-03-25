using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using DG.Tweening;
/// <summary>
/// 【C#】2次元配列の宣言・初期化・代入
/// https://algorithm.joho.info/programming/csharp/2d-array-cs/
/// 【C#,LINQ】Select,SelectMany～配列やリスト内の要素の形を変形したいとき～
/// https://www.urablog.xyz/entry/2018/05/28/070000
/// 配列で渡された要素をグループに分ける
/// https://qiita.com/masaru/items/aaf39f5b1b31ee53bd23
/// 【C#,LINQ】GroupBy～配列やリストをグループ化したいとき～
/// https://www.urablog.xyz/entry/2018/07/07/070000
/// 配列やコレクションをシャッフルする（ランダムに並び替える）
/// https://dobon.net/vb/dotnet/programing/arrayshuffle.html
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

        int lineNum = 2;
        for (int i = 1; i < lineNum + 1; i++)
        {
            SetBlocksNewLine(i);
        }

    }

    public BlockController GetBlock(int indexX, int indexY)
    {
        return blockControllers
            .Where(b => b.indexX == indexX)
            .Where(b => b.indexY == indexY)
            .Where(b => b.gameObject.activeSelf)
            .FirstOrDefault();
    }

    public BlockController GetDragBlock()
    {
        return blockControllers
            .Where(block => block.gameObject.activeSelf)
            .Where(block => block.blockState == BlockState.DRAG)
            .FirstOrDefault();
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
    Tween tween;
    public void AutoMergeBlocks()
    {
        Variables.gameState = GameState.AUTO_MERGE;

        tween = DOVirtual.DelayedCall(0.1f, () =>
        {
            AutoMerge();
        })
        .SetLoops(-1);

    }

    void AutoMerge()
    {
        List<BlockController> blocks = blockControllers
            .Where(block => block.gameObject.activeSelf)
            .ToList();

        var group = blocks
            .GroupBy(block => block.num)
            .OrderBy(g => g.Key)
            .Where(g => g.Count() > 1)
            .FirstOrDefault();

        if (group == null)
        {
            tween.Kill();
            Variables.gameState = GameState.IN_PROGRESS_TIMER;
            return;
        }

        List<BlockController> duplicateBlocks = group
            .Select(b => b)
            .ToList();

        if (duplicateBlocks.Count < 2) { return; }

        int x = duplicateBlocks[1].indexX;
        int y = duplicateBlocks[1].indexY;
        duplicateBlocks[0].TransrateBlock(x, y);

    }

    public void EraseLine(int indexY)
    {
        var blocks = blockControllers
            .Where(b => b.indexY == indexY)
            .Where(b => b.gameObject.activeSelf)
            .ToList();

        for (int i = 0; i < blocks.Count; i++)
        {
            blocks[i].gameObject.SetActive(false);
        }
    }

}
