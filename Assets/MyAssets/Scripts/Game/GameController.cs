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
        Variables.sumOfErasedBlockNumbers = 0;
        blocksManager.OnInitialize();
        Variables.isDragging = false;
        Variables.gameState = GameState.IN_PROGRESS_TIMER;
    }

    public void OnUpdate()
    {
        blocksManager.OnUpdate();

        switch (Variables.gameState)
        {
            case GameState.IN_PROGRESS_TIMER:
                Variables.timer -= Time.deltaTime;
                ForceResetTimer();
                CheckTimer();
                break;
            case GameState.RESET_TIMER:
                blocksManager.SetBlocksNewLine(0);
                blocksManager.MoveUpAllBlocks();
                Variables.timer = Values.TIME_LIMIT;
                Variables.gameState = GameState.MOVE_UP_ANIM;
                break;
            case GameState.MOVE_UP_ANIM:
                break;
        }
    }

    void CheckTimer()
    {
        if (Variables.timer > 0) { return; }
        if (!blocksManager.IsAllBlockStopped()) { return; }
        Variables.gameState = GameState.RESET_TIMER;
    }

    void ForceResetTimer()
    {
        if (blocksManager.IsDuplicateBlockNum()) { return; }
        Variables.timer = -1f;
    }

}
