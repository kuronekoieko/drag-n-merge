using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class StartCanvasManager : MonoBehaviour
{
    [SerializeField] Button startButton;
    public void OnStart()
    {
        startButton.onClick.AddListener(OnClickStartButton);
        gameObject.SetActive(true);
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
