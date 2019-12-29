using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] BlocksManager blocksManager;
    float timeLimit = 10;
    public void OnStart()
    {
        blocksManager.OnStart();
        Variables.timer = timeLimit;
    }

    public void OnUpdate()
    {
        blocksManager.OnUpdate();
        Variables.timer -= Time.deltaTime;
        if (Variables.timer < 0)
        {
            blocksManager.SetBlocksNewLine();
            Variables.timer = timeLimit;
        }
    }
}
