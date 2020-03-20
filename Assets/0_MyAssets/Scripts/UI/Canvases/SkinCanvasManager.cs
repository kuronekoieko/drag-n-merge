using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Linq;

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
    public readonly ScreenState thisScreen = ScreenState.SKIN;

    public override void OnStart()
    {
        base.SetScreenAction(thisScreen: thisScreen);
        closeButton.onClick.AddListener(OnClickCloseButton);

        ToggleGanarator();

    }

    public override void OnInitialize()
    {
        gameObject.SetActive(false);
    }

    protected override void OnOpen()
    {
        FirebaseAnalyticsManager.i.LogEvent("画面_スキン");
        gameObject.SetActive(true);
        for (int i = 0; i < bGToggleControllers.Length; i++)
        {
            bGToggleControllers[i].OnOpen();
        }

        OpenHiddenLock();
    }

    public void OpenHiddenLock()
    {
        int shownLockCount = SaveData.i.possessedBackgrounds
                    .Where(p => p.bGToggleState == BGToggleState.ShownLock)
                    .Count();

        if (shownLockCount != 0) { return; }

        int firstHiddenIndex = SaveData.i.possessedBackgrounds
                .FindIndex(p => p.bGToggleState == BGToggleState.HiddenLock);

        if (firstHiddenIndex == -1) { return; }

        int lastOpenIndex = firstHiddenIndex + 8;
        int lastIndex = SaveData.i.possessedBackgrounds.Count - 1;
        if (lastOpenIndex > lastIndex)
        {
            lastOpenIndex = lastIndex;
        }

        for (int i = firstHiddenIndex; i < lastOpenIndex + 1; i++)
        {
            SaveData.i.possessedBackgrounds[i].bGToggleState = BGToggleState.ShownLock;
        }
        SaveDataManager.i.Save();

    }

    protected override void OnClose()
    {
        gameObject.SetActive(false);
    }

    void OnClickCloseButton()
    {
        Variables.screenState = Variables.lastScreenState;
        GameManager.i.gameController.backgroundController.SetBackground();
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
                index: i,
                skinCanvasManager: this);
        }
    }
}
