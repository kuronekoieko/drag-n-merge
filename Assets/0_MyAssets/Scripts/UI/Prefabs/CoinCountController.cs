using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;

public class CoinCountController : MonoBehaviour
{
    [SerializeField] Text coinCountText;
    [SerializeField] Button openShopButton;
    int coinCount;
    void Start()
    {
        this.ObserveEveryValueChanged(coinCount => SaveData.i.coinCount)
            .Subscribe(coinCount => CountUpAnim())
            .AddTo(this.gameObject);

        this.ObserveEveryValueChanged(coinCount => this.coinCount)
            .Subscribe(coinCount => coinCountText.text = coinCount.ToString())
            .AddTo(this.gameObject);

        openShopButton.onClick.AddListener(OnClickShopButton);
    }

    void CountUpAnim()
    {
        DOTween.To(() => coinCount, (x) => coinCount = x, SaveData.i.coinCount, 0.5f);
    }

    void OnClickShopButton()
    {
        SaveData.i.coinCount += 100;
        SaveDataManager.i.Save();
    }
}
