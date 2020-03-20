using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class CoinAnimController : MonoBehaviour
{
    RectTransform targetRT;
    RectTransform rectTransform;
    float endScale = 0.7f;

    public void OnStart(RectTransform targetRT)
    {
        rectTransform = GetComponent<RectTransform>();
        this.targetRT = targetRT;
    }

    public void MoveTowardsCoin(float delayTime, Action OnComplete = null)
    {
        float duration = 1f;
        Sequence sequence = DOTween.Sequence()
            .AppendInterval(delayTime)
            .Append(rectTransform.DOLocalMove(targetRT.anchoredPosition, duration).SetEase(Ease.InBack))
            .OnComplete(() => { if (OnComplete != null) { OnComplete(); } });

        rectTransform.DOScale(Vector3.one * endScale, duration).SetEase(Ease.InBack);

    }
}
