﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UniRx;
using System;

public class ItemButtonController : MonoBehaviour
{
    [SerializeField] Image bgImage;
    [SerializeField] Text countText;
    [SerializeField] Image badgeImage;
    [SerializeField] Sprite[] itemSprites;
    [SerializeField] RectTransform coinCountRT;
    [SerializeField] Text coinCountText;
    Button button;
    RectTransform rectTransform;
    int index;
    int coinCount;

    public void OnStart(int index, Vector3 pos)
    {
        this.index = index;
        bgImage.sprite = itemSprites[index];
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = pos;
        button = GetComponent<Button>();

        this.ObserveEveryValueChanged(itemCount => SaveData.i.itemCounts[index])
            .Subscribe(itemCount => ShowCounts(itemCount))
            .AddTo(this.gameObject);

        var buttonActions = new List<UnityAction>()
        {
            OnClickAddTimeButton,
            OnClickFallBlockButton,
            OnClickShuffleButton,
            OnClickAutoMergeButton,
        };

        button.onClick.AddListener(() =>
        {
            UseItem(() => { buttonActions[index](); });
        });

        coinCount = ItemDataSO.i.itemDatas[index].coinCount;
        coinCountText.text = coinCount.ToString();
    }

    void ShowCounts(int itemCount)
    {
        countText.text = itemCount.ToString();
        bool isShowCoin = (itemCount == 0);
        badgeImage.gameObject.SetActive(!isShowCoin);
        coinCountRT.gameObject.SetActive(isShowCoin);
    }

    void OnClickAddTimeButton()
    {
        Variables.timer += 5;
    }

    void OnClickFallBlockButton()
    {
        BlocksManager.i.ShowBlocksTopLine();
    }

    void OnClickShuffleButton()
    {
        BlocksManager.i.ShuffleBlocks();
    }

    void OnClickAutoMergeButton()
    {
        BlocksManager.i.AutoMergeBlocks();
    }

    void UseItem(Action ItemAction)
    {
        bool isNotEnoughItem = (SaveData.i.itemCounts[index] < 1);
        bool isNotEnoughCoin = (SaveData.i.coinCount < coinCount);

        if (isNotEnoughItem == false)
        {
            if (!BlocksManager.i.IsAllBlockStopped()) { return; }
            ItemAction();
            SaveData.i.itemCounts[index]--;
            SaveDataManager.i.Save();
            return;
        }

        if (isNotEnoughCoin == false)
        {
            if (!BlocksManager.i.IsAllBlockStopped()) { return; }
            ItemAction();
            SaveData.i.coinCount -= coinCount;
            SaveDataManager.i.Save();
            return;
        }

    }



}