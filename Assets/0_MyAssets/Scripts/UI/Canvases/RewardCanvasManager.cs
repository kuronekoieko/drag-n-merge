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
    float blockScale;

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
        blockScale = blockRT.localScale.x;
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
        // 前半----------------------------
        ChangeAlpha(bgImage, 0);
        bgImage.DOFade(bgColor.a, 0.5f);

        ChangeAlpha(haloImage, 0);
        haloImage.DOFade(1, 1f);


        blockRT.localScale = Vector3.zero;
        blockRT.DOScale(Vector3.one * blockScale, 0.5f).SetEase(Ease.OutBack);

        text.rectTransform.localScale = Vector3.zero;
        text.rectTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

        float delayTime = 0.5f;
        for (int i = 0; i < coinAnims.Length; i++)
        {
            coinAnims[i].Show(0.5f, delayTime);
            delayTime += 0.05f;
        }

        // 後半----------------------------
        claimButtonRT.localScale = Vector3.zero;
        claimButtonRT.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetDelay(0.5f);

        coinCountText.rectTransform.localScale = Vector3.zero;
        coinCountText.rectTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetDelay(0.5f);
    }

    void HideAnims()
    {

        // 前半----------------------------
        haloImage.DOFade(0, 1f);
        blockRT.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
        text.rectTransform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);

        // 後半----------------------------
        bgImage.DOFade(0, 0.5f).SetDelay(0.5f);
        claimButtonRT.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).SetDelay(0.5f);
        coinCountText.rectTransform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).SetDelay(0.5f);

        DOVirtual.DelayedCall(1f, () =>
        {
            gameObject.SetActive(false);
            Variables.screenState = ScreenState.GAME;
        });
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

        HideAnims();

    }

}
