using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ItemButtonController : MonoBehaviour
{
    [SerializeField] Image bgImage;
    [SerializeField] Text countText;
    [SerializeField] Image badgeImage;
    Button button;
    RectTransform rectTransform;
    public void OnStart(Sprite bgSprite, Vector3 pos, UnityAction OnClickButton)
    {
        countText.text = "0";
        bgImage.sprite = bgSprite;
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = pos;
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClickButton);
    }


}
