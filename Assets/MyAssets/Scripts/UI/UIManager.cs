﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UIManager : MonoBehaviour
{

    [SerializeField] RectTransform canvasesParent;
    BaseCanvasManager[] canvasManagers;
    public void OnStart()
    {
        canvasManagers = new BaseCanvasManager[canvasesParent.childCount];
        for (int i = 0; i < canvasManagers.Length; i++)
        {
            canvasManagers[i] = canvasesParent.GetChild(i).GetComponent<BaseCanvasManager>();
            if (canvasManagers[i] == null) { continue; }
            canvasManagers[i].OnStart();
        }
    }

    public void OnInitialize()
    {
        for (int i = 0; i < canvasManagers.Length; i++)
        {
            if (canvasManagers[i] == null) { continue; }
            canvasManagers[i].OnInitialize();
        }
    }
}
