using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] BlocksManager blocksManager;

    public void OnStart()
    {
        blocksManager.OnStart();
    }

    public void OnInitialize()
    {
        Variables.timer = Values.TIME_LIMIT;
        Variables.eraseTargetBlockCount = 0;
        blocksManager.OnInitialize();
        Variables.isDragging = false;
    }

    public void OnUpdate()
    {
        blocksManager.OnUpdate();

        if (Variables.timer < 0)
        {
            IsUpNewLine();
        }
        else
        {
            Variables.timer -= Time.deltaTime;
            ResetTimer();
        }
    }

    void IsUpNewLine()
    {
        if (Variables.isDragging) { return; }

        blocksManager.SetBlocksNewLine(0);
        blocksManager.MoveUpAllBlocks();
        Variables.timer = Values.TIME_LIMIT;

    }

    void ResetTimer()
    {
        if (Variables.isDragging) { return; }
        if (blocksManager.IsDuplicateBlockNum()) { return; }
        Variables.timer = -1f;
        //Debug.Log("リセット");
    }

}
