using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugCanvasManager : BaseCanvasManager
{
    [SerializeField] Button openButton;
    [SerializeField] Button hideButton;
    [SerializeField] Image bannerImage;
    [SerializeField] Image debugPanel;
    [SerializeField] Button applyButton;
    [SerializeField] Button cancelButton;
    [SerializeField] InputField stageNumIF;
    [SerializeField] Toggle isDebugStageT;

    public override void OnStart()
    {
        gameObject.SetActive(Debug.isDebugBuild);
        debugPanel.gameObject.SetActive(false);
        openButton.onClick.AddListener(OnClickOpenButton);
        hideButton.onClick.AddListener(OnClickHideButton);
        applyButton.onClick.AddListener(OnClickApplyButton);
        cancelButton.onClick.AddListener(OnClickCancelButton);
    }

    public override void OnInitialize()
    {
    }

    void OnClickOpenButton()
    {
        debugPanel.gameObject.SetActive(true);
        ShowParam();
    }

    void ShowParam()
    {
        stageNumIF.text = Variables.adStageIndex.ToString();
        isDebugStageT.isOn = Variables.isDebugStage;
    }

    void OnClickHideButton()
    {
        bool activeSelf = bannerImage.gameObject.activeSelf;
        bannerImage.gameObject.SetActive(!activeSelf);
        hideButton.GetComponent<CanvasGroup>().alpha = activeSelf ? 0 : 1;
    }

    void OnClickApplyButton()
    {
        if (int.TryParse(stageNumIF.text, out int adStageIndex))
        {
            Variables.adStageIndex = adStageIndex;
        }

        Variables.isDebugStage = isDebugStageT.isOn;

        Variables.screenState = ScreenState.INITIALIZE;
        Close();
    }

    void OnClickCancelButton()
    {
        Close();
    }

    void Close()
    {
        debugPanel.gameObject.SetActive(false);
    }
}
