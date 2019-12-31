using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] BlocksManager blocksManager;
    float timeLimit = 10;
    public void OnStart()
    {
        Variables.targetNum = 13;
        blocksManager.OnStart();
        Variables.timer = timeLimit;
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
