using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;

public class StartCanvasManager : MonoBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] Text targetNumText;
    [SerializeField] Image targetBlockImage;
    [SerializeField] Text startText;

    public void OnStart()
    {
        startButton.onClick.AddListener(OnClickStartButton);
        gameObject.SetActive(true);
        targetNumText.text = Variables.targetNum.ToString();
        targetNumText.color = BlockColorData.i.blockColors[Variables.targetNum - 1].textColor;
        targetBlockImage.color = BlockColorData.i.blockColors[Variables.targetNum - 1].color;

        Anim();
    }

    void Anim()
    {
        startText.transform.DOScale(1.1f, 0.5f)
               .OnComplete(() =>
               {
                   startText.transform.DOScale(1f, 0.5f)
                           .OnComplete(() =>
                           {
                               Anim();
                           });
               });
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
