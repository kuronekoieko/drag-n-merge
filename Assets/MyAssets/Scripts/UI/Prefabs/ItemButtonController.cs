using System.Collections;
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
    Button button;
    RectTransform rectTransform;
    int index;

    public void OnStart(int index, Vector3 pos)
    {
        this.index = index;
        bgImage.sprite = itemSprites[index];
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = pos;
        button = GetComponent<Button>();

        this.ObserveEveryValueChanged(itemCount => SaveData.i.itemCounts[index])
            .Subscribe(itemCount => { countText.text = itemCount.ToString(); })
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
        if (SaveData.i.itemCounts[index] < 1) { return; }
        if (!BlocksManager.i.IsAllBlockStopped()) { return; }
        ItemAction();
        SaveData.i.itemCounts[index]--;
        SaveDataManager.i.Save();
    }

}
