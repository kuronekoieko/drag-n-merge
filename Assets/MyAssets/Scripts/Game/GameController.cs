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

    public void OnUpdate()
    {
        blocksManager.OnUpdate();
    }
}
