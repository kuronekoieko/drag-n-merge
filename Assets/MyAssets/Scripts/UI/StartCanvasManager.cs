using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class StartCanvasManager : MonoBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] Text targetNumText;
    [SerializeField] Image targetBlockImage;
    public void OnStart()
    {
        startButton.onClick.AddListener(OnClickStartButton);
        gameObject.SetActive(true);
        targetNumText.text = Variables.targetNum.ToString();
        targetNumText.color = BlockColorData.i.blockColors[Variables.targetNum - 1].textColor;
        targetBlockImage.color = BlockColorData.i.blockColors[Variables.targetNum - 1].color;
    }

    void OnOpen()
    {

    }

    void OnClickStartButton()
    {
        gameObject.SetActive(false);
        Variables.screenState = ScreenState.GAME;
        AudioManager.i.PlayOneShot(1);
    }
}
