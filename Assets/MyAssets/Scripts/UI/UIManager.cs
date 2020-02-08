using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            canvasManagers[i].OnStart();
        }
    }

    public void OnInitialize()
    {
        for (int i = 0; i < canvasManagers.Length; i++)
        {
            canvasManagers[i].OnInitialize();
        }
    }
}
