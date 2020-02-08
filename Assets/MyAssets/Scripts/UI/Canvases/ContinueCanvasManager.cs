using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class ContinueCanvasManager : BaseCanvasManager
{
    [SerializeField] Button coinButton;
    [SerializeField] Button videoButton;
    [SerializeField] Button cancelButton;
    [SerializeField] Text countText;
    [SerializeField] Text coinCountText;
    [SerializeField] Image circleImage;

    float timer;
    float startTime = 9;
    int coinCount = 200;

    public override void OnStart()
    {
        base.SetScreenAction(thisScreen: ScreenState.CONTINUE);

        coinButton.onClick.AddListener(OnClickCoinButton);
        videoButton.onClick.AddListener(OnClickVideoButton);
        cancelButton.onClick.AddListener(OnClickCancelButton);
        coinCountText.text = coinCount.ToString();
    }

    public override void OnInitialize()
    {
        gameObject.SetActive(false);
        timer = startTime;
    }

    void Update()
    {
        if (Variables.screenState != ScreenState.CONTINUE) { return; }

        if (timer < 0)
        {
            Variables.screenState = ScreenState.RESULT;
            return;
        }
        timer -= Time.deltaTime;
        countText.text = Mathf.CeilToInt(timer).ToString();
        circleImage.fillAmount = timer / startTime;
    }

    protected override void OnOpen()
    {
        gameObject.SetActive(true);
    }

    protected override void OnClose()
    {
        gameObject.SetActive(false);
    }

    void OnClickCoinButton()
    {
        if (SaveData.i.coinCount < coinCount) { return; }
        SaveData.i.coinCount -= coinCount;
        SaveDataManager.i.Save();
        Continue();
    }

    void OnClickVideoButton()
    {
        Continue();
    }

    void OnClickCancelButton()
    {
        Variables.screenState = ScreenState.RESULT;
    }

    void Continue()
    {
        for (int i = 1; i < 6; i++)
        {
            BlocksManager.i.EraseLine(i);
        }

        Variables.screenState = ScreenState.GAME;
    }

}
