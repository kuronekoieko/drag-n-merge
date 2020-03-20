using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RewardCanvasManager : BaseCanvasManager
{
    [SerializeField] Button claimButton;
    [SerializeField] Text coinCountText;
    [SerializeField] Image haloImage;
    int coinCount;

    public override void OnStart()
    {
        base.SetScreenAction(thisScreen: ScreenState.REWARD);
        gameObject.SetActive(false);
        claimButton.onClick.AddListener(OnClickClaimButton);

        haloImage.rectTransform.DOLocalRotate(new Vector3(0, 0, -360), 6)
            .SetRelative()
            .SetLoops(-1)
            .SetEase(Ease.Linear);
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
