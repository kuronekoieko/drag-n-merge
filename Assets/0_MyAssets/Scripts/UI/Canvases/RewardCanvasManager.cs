using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using System;

public class RewardCanvasManager : BaseCanvasManager
{
    [SerializeField] Button claimButton;
    [SerializeField] Text coinCountText;
    [SerializeField] Image haloImage;
    [SerializeField] CoinAnimController[] coinAnims;
    [SerializeField] RectTransform coinTargetRT;
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

        for (int i = 0; i < coinAnims.Length; i++)
        {
            coinAnims[i].OnStart(coinTargetRT);
        }
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
        CoinMoveAnims();
    }

    void CoinMoveAnims()
    {
        float delayTime = 0;
        Action OnComplete = null;
        for (int i = 0; i < coinAnims.Length; i++)
        {
            if (i == coinAnims.Length - 1)
            {
                OnComplete = OnCompleteCoinMoveAnim;
            }
            coinAnims[i].MoveTowardsCoin(delayTime, OnComplete);
            delayTime += 0.1f;
        }
    }

    void OnCompleteCoinMoveAnim()
    {
        SaveData.i.coinCount += coinCount;
        SaveDataManager.i.Save();

        gameObject.SetActive(false);
    }

}
