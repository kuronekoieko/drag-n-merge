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
    Vector3 startPos;
    float endScale = 0.7f;

    public void OnStart(RectTransform targetRT)
    {
        rectTransform = GetComponent<RectTransform>();
        this.targetRT = targetRT;
        startPos = rectTransform.anchoredPosition;
    }

    public void MoveTowardsCoin(float delayTime, Action OnComplete = null)
    {
        float duration = 0.7f;

        rectTransform
            .DOLocalMove(targetRT.anchoredPosition, duration)
            .SetEase(Ease.InBack)
            .SetDelay(delayTime)
            .OnComplete(() => { if (OnComplete != null) { OnComplete(); } });

        rectTransform.DOScale(Vector3.one * endScale, duration).SetEase(Ease.InBack);

    }

    public void Show(float duration, float delayTime)
    {
        rectTransform.anchoredPosition = startPos;
        rectTransform.localScale = Vector3.zero;

        rectTransform
            .DOScale(Vector3.one, duration)
            .SetEase(Ease.OutBack)
            .SetDelay(delayTime);
    }
}
