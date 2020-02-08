using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UniRx;

public class ItemButtonController : MonoBehaviour
{
    [SerializeField] Image bgImage;
    [SerializeField] Text countText;
    [SerializeField] Image badgeImage;
    [SerializeField] Sprite[] itemSprites;
    Button button;
    RectTransform rectTransform;
    public void OnStart(int index, Vector3 pos)
    {
        bgImage.sprite = itemSprites[index];
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = pos;
        button = GetComponent<Button>();

        this.ObserveEveryValueChanged(itemCount => SaveData.i.itemCounts[index])
            .Subscribe(itemCount => { countText.text = itemCount.ToString(); })
            .AddTo(this.gameObject);
    }

    public void SetButtonAction(UnityAction OnClickButton)
    {
        button.onClick.AddListener(OnClickButton);
    }


}
