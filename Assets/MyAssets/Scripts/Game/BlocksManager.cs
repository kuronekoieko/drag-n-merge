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
        Vector2 pos;
        int i_x = 0;
        int i_y = 0;
        for (int i = 0; i < blockControllers.Length; i++)
        {
            pos.x = -2.47f + i_x * 0.16f * 6;
            pos.y = -4.05f + i_y * 0.16f * 6;
            //Debug.Log(pos);
            blockControllers[i] = Instantiate(blockControllerPrefab, pos, Quaternion.identity, transform);
            blockControllers[i].OnStart();

            i_x++;
            if (i_x == Values.BOARD_LENGTH_X)
            {
                i_x = 0;
                i_y++;
            }

        }
    }
}
