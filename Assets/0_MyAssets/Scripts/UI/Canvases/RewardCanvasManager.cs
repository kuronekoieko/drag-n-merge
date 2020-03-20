using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardCanvasManager : BaseCanvasManager
{
    [SerializeField] Button claimButton;
    [SerializeField] Text coinCountText;
    int coinCount;

    public override void OnStart()
    {
        base.SetScreenAction(thisScreen: ScreenState.REWARD);
        gameObject.SetActive(false);
        claimButton.onClick.AddListener(OnClickClaimButton);
    }

    public override void OnInitialize()
    {
    }

    protected override void OnOpen()
    {
        gameObject.SetActive(true);

        coinCount = Utils.GetMasterData(Variables.eraseTargetBlockCount).coinCount;


        coinCountText.text = "+ " + coinCount;
    }

    protected override void OnClose()
    {
        gameObject.SetActive(false);
    }

    void OnClickClaimButton()
    {
        SaveData.i.coinCount += coinCount;
        SaveDataManager.i.Save();

        gameObject.SetActive(false);
    }

}
