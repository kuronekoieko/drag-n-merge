using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UIManager : MonoBehaviour
{

    [SerializeField] RectTransform canvasesParent;
    BaseCanvasManager[] canvasManagers;
    ScreenState lastScreenState;
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

    public void Change(ScreenState open, ScreenState close = ScreenState.NONE)
    {
        ScreenState currentScreenState = Variables.screenState;
        if (GetCanvas(close, out BaseCanvasManager closeCanvas))
        {
            closeCanvas.Close();
            Variables.screenState = lastScreenState;
        }

        if (GetCanvas(open, out BaseCanvasManager openCanvas))
        {
            openCanvas.Open();
            lastScreenState = currentScreenState;
            Variables.screenState = open;
        }
    }

    void Close(ScreenState screenState)
    {
        var canvas = canvasManagers
                    .Where(c => c.screenState == screenState)
                    .FirstOrDefault();
        if (canvas == null) { return; }
        canvas.Close();
        Variables.screenState = lastScreenState;
    }

    void Open(ScreenState screenState)
    {
        var canvas = canvasManagers
                    .Where(c => c.screenState == screenState)
                    .FirstOrDefault();
        if (canvas == null) { return; }
        canvas.Open();

        lastScreenState = Variables.screenState;
        Variables.screenState = screenState;
    }

    bool GetCanvas(ScreenState screenState, out BaseCanvasManager baseCanvas)
    {
        baseCanvas = canvasManagers
            .Where(c => c.screenState == screenState)
            .FirstOrDefault();
        return baseCanvas;
    }

}
