﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class SkinCanvasManager : BaseCanvasManager
{
    /*
    【Unity uGUI】Toggleコンポーネントを徹底解説
    https://tech.pjin.jp/blog/2017/01/29/unity_ugui_toggle/#Group
    uGUIのScroll ViewでGrid
    https://qiita.com/okuhiiro/items/d33a5c8940bf0843d8ff
    */
    [SerializeField] Button closeButton;
    [SerializeField] ToggleGroup toggleGroup;
    [SerializeField] BackgroundToggleController backgroundToggleControllerPrefab;
    BackgroundToggleController[] bGToggleControllers;

    public override void OnStart()
    {
        base.SetScreenAction(thisScreen: ScreenState.SKIN);
        closeButton.onClick.AddListener(OnClickCloseButton);

        ToggleGanarator();

    }

    public override void OnInitialize()
    {
        gameObject.SetActive(false);
    }

    protected override void OnOpen()
    {
        gameObject.SetActive(true);
        for (int i = 0; i < bGToggleControllers.Length; i++)
        {
            bGToggleControllers[i].OnOpen();
        }
    }

    protected override void OnClose()
    {
        gameObject.SetActive(false);
    }

    void OnClickCloseButton()
    {
        Variables.screenState = Variables.lastScreenState;
        GameManager.i.gameController.SetBackground();
        AudioManager.i.PlayOneShot(5);
    }

    void ToggleGanarator()
    {
        RectTransform content = toggleGroup.GetComponent<RectTransform>();
        bGToggleControllers = new BackgroundToggleController[BackgroundDataSO.i.backgroundDatas.Length];
        for (int i = 0; i < bGToggleControllers.Length; i++)
        {
            bGToggleControllers[i] = Instantiate(backgroundToggleControllerPrefab, Vector3.zero, Quaternion.identity, content);
            bGToggleControllers[i].OnStart(
                toggleGroup: toggleGroup,
                index: i);
        }
    }
}
