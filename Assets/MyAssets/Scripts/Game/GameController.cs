using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] BlocksManager blocksManager;
    float timeLimit = 1;
    public void OnStart()
    {
        Variables.targetNum = 3;
        blocksManager.OnStart();
        Variables.timer = timeLimit;
        Variables.isDragging = false;
    }

    public void OnInitialize()
    {
        blocksManager.OnInitialize();
    }

    public void OnUpdate()
    {
        blocksManager.OnUpdate();

        if (Variables.timer < 0)
        {
            if (!Variables.isDragging)
            {
                blocksManager.SetBlocksNewLine(0);
                blocksManager.MoveUpAllBlocks();
                Variables.timer = timeLimit;
            }
        }
        else
        {
            Variables.timer -= Time.deltaTime;
        }
    }
}
