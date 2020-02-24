using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class SkinCanvasManager : BaseCanvasManager
{
    [SerializeField] Button closeButton;
    [SerializeField] RectTransform content;
    [SerializeField] BackgroundToggleController backgroundToggleControllerPrefab;
    BackgroundToggleController[] bGToggleControllers;
    public override void OnStart()
    {
        base.SetScreenAction(thisScreen: ScreenState.SKIN);
        closeButton.onClick.AddListener(OnClickCloseButton);

        bGToggleControllers = new BackgroundToggleController[BackgroundDataSO.i.backgroundDatas.Length];
        for (int i = 0; i < bGToggleControllers.Length; i++)
        {
            bGToggleControllers[i] = Instantiate(backgroundToggleControllerPrefab, Vector3.zero, Quaternion.identity, content);
            bGToggleControllers[i].OnStart(BackgroundDataSO.i.backgroundDatas[i]);
        }

    }

    public override void OnInitialize()
    {
        gameObject.SetActive(false);
    }

    protected override void OnOpen()
    {
        gameObject.SetActive(true);
    }

    protected override void OnClose()
    {
        gameObject.SetActive(false);
    }

    void OnClickCloseButton()
    {
        Variables.screenState = ScreenState.START;
    }
}
