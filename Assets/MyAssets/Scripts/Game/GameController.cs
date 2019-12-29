using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] BlocksManager blocksManager;
    public void OnStart()
    {
        blocksManager.OnStart();
        Variables.timer = 10;
    }

    public void OnUpdate()
    {
        blocksManager.OnUpdate();
        Variables.timer -= Time.deltaTime;
        if (Variables.timer < 0)
        {
            Variables.timer = 10;
        }
    }
}
