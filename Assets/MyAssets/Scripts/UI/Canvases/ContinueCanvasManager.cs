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

    public override void OnStart()
    {
        base.SetScreenAction(thisScreen: ScreenState.CONTINUE);

        coinButton.onClick.AddListener(OnClickCoinButton);
        videoButton.onClick.AddListener(OnClickVideoButton);
        cancelButton.onClick.AddListener(OnClickCancelButton);
    }



    public override void OnInitialize()
    {
        gameObject.SetActive(false);
        timer = 9;
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
