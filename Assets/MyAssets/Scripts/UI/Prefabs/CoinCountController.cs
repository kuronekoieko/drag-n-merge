using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class CoinCountController : MonoBehaviour
{
    [SerializeField] Text coinCountText;
    [SerializeField] Button openShopButton;

    void Start()
    {
        this.ObserveEveryValueChanged(coinCount => SaveData.i.coinCount)
            .Subscribe(coinCount => coinCountText.text = coinCount.ToString())
            .AddTo(this.gameObject);

        openShopButton.onClick.AddListener(OnClickShopButton);
    }

    void OnClickShopButton()
    {
        SaveData.i.coinCount += 100;
        SaveDataManager.i.Save();
    }
}
