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

    public override void OnStart()
    {
        base.SetScreenAction(thisScreen: ScreenState.CONTINUE);
    }

    public override void OnInitialize()
    {
    }

    protected override void OnOpen()
    {
    }

    protected override void OnClose()
    {
    }
}
