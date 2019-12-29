﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] BlockController blockController;
    public void OnStart()
    {
        blockController.OnStart();
    }

    public void OnUpdate()
    {
        blockController.OnUpdate();
    }
}
