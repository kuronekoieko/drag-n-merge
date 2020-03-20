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
    [SerializeField] Image bgImage;
    [SerializeField] RectTransform blockRT;
    [SerializeField] Text text;
    [SerializeField] RectTransform claimButtonRT;

    int coinCount;
    Color bgColor;

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

        bgColor = bgImage.color;

    }

    void ChangeAlpha(Image image, float a)
    {
        Color color = image.color;
        color.a = a;
        image.color = color;
    }

    public override void OnInitialize()
    {
    }

    protected override void OnOpen()
    {
        gameObject.SetActive(true);

        coinCount = Utils.GetMasterData(Variables.eraseTargetBlockCount).coinCount;
        coinCountText.text = "+ " + coinCount;

        claimButton.enabled = true;
        ShowAnims();
    }

    void ShowAnims()
    {
        ChangeAlpha(bgImage, 0);
        bgImage.DOFade(bgColor.a, 0.5f);

        ChangeAlpha(haloImage, 0);
        haloImage.DOFade(1, 1f);

        float scale = blockRT.localScale.x;
        blockRT.localScale = Vector3.zero;
        blockRT.DOScale(Vector3.one * scale, 0.5f).SetEase(Ease.OutBack);

        text.rectTransform.localScale = Vector3.zero;
        text.rectTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

        float delayTime = 0.2f;
        for (int i = 0; i < coinAnims.Length; i++)
        {
            coinAnims[i].Show(0.5f, delayTime);
            delayTime += 0.05f;
        }

        claimButtonRT.localScale = Vector3.zero;
        claimButtonRT.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetDelay(0.2f);

    }

    protected override void OnClose()
    {
        gameObject.SetActive(false);
    }

    void OnClickClaimButton()
    {
        claimButton.enabled = false;
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
        Variables.screenState = ScreenState.GAME;
    }

}
