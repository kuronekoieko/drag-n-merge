using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlocksManager : MonoBehaviour
{
    [SerializeField] BlockController blockControllerPrefab;
    BlockController[] blockControllers;
    public void OnStart()
    {
        BlockGenerator();

        for (int i = 0; i < 2; i++)
        {
            SetBlocksNewLine();
            MoveUpAllBlocks();
        }
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

    void SetBlocksNewLine()
    {
        Vector2 pos = new Vector2(-2.5f, -3.6f);
        int count = 0;

        for (int i = 0; i < blockControllers.Length; i++)
        {
            BlockController block = blockControllers[i];
            if (block.gameObject.activeSelf) { continue; }

            block.transform.position = pos;
            pos.x += 0.16f * 6;
            block.gameObject.SetActive(true);
            count++;
            if (count == Values.BOARD_LENGTH_X) { break; }
        }
    }

    void MoveUpAllBlocks()
    {
        for (int i = 0; i < blockControllers.Length; i++)
        {
            BlockController block = blockControllers[i];
            Vector2 pos = block.transform.position;
            pos.y += 0.16f * 6;
            block.transform.position = pos;
        }
    }
}
