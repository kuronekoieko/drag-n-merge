using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] BlocksManager blocksManager;
    float timeLimit;
    public void OnStart()
    {
        blocksManager.OnStart();
    }

    public void OnInitialize()
    {
        timeLimit = 1;
        Variables.timer = timeLimit;
        Variables.eraseTargetBlockCount = 0;
        blocksManager.OnInitialize();
        Variables.isDragging = false;
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
