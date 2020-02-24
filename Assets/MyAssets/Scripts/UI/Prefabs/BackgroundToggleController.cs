using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class BackgroundToggleController : MonoBehaviour
{
    [SerializeField] Image bGImage;
    [SerializeField] Text coinCountText;
    Toggle toggle;

    public void OnStart(BackgroundData backgroundData)
    {
        bGImage.sprite = backgroundData.sprite;
        coinCountText.text = backgroundData.coinCount.ToString();
        toggle = GetComponent<Toggle>();


    }

}
