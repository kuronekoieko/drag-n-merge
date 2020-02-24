using System.Collections;
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
    [SerializeField] SpriteRenderer bGSpriteRenderer;
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
            bGToggleControllers[i].isOn = (i == 0);
        }
    }

    protected override void OnClose()
    {
        gameObject.SetActive(false);
        for (int i = 0; i < bGToggleControllers.Length; i++)
        {
            var bGToggle = bGToggleControllers[i];
            if (bGToggle.isOn)
            {
                bGSpriteRenderer.sprite = bGToggle.backgroundData.sprite;
            }
        }
    }

    void OnClickCloseButton()
    {
        Variables.screenState = ScreenState.START;
    }

    void ToggleGanarator()
    {
        RectTransform content = toggleGroup.GetComponent<RectTransform>();
        bGToggleControllers = new BackgroundToggleController[BackgroundDataSO.i.backgroundDatas.Length];
        for (int i = 0; i < bGToggleControllers.Length; i++)
        {
            bGToggleControllers[i] = Instantiate(backgroundToggleControllerPrefab, Vector3.zero, Quaternion.identity, content);
            bGToggleControllers[i].OnStart(BackgroundDataSO.i.backgroundDatas[i], toggleGroup);
        }
    }
}
