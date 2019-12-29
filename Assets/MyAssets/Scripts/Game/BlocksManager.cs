using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlocksManager : MonoBehaviour
{
    [SerializeField] BlockController[] blockControllers;
    public void OnStart()
    {
        for (int i = 0; i < blockControllers.Length; i++)
        {
            blockControllers[i].OnStart();
        }

    }

    public void OnUpdate()
    {
        for (int i = 0; i < blockControllers.Length; i++)
        {
            blockControllers[i].OnUpdate();
        }
    }
}
