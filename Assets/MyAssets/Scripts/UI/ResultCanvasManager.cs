using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class ResultCanvasManager : MonoBehaviour
{
    [SerializeField] Text resultText;
    [SerializeField] Button restartButton;
    public void OnStart()
    {

        this.ObserveEveryValueChanged(screenState => Variables.screenState)
            .Where(screenState => screenState == ScreenState.RESULT)
            .Subscribe(timer => { OnOpen(); })
            .AddTo(this.gameObject);

        restartButton.onClick.AddListener(OnClickRestartButton);

        gameObject.SetActive(false);
    }

    void OnOpen()
    {
        gameObject.SetActive(true);
        resultText.text = Variables.resultState == ResultState.WIN ? "CLEAR!!" : "FAILED";
        if (Variables.resultState == ResultState.WIN)
        {
            resultText.text = "CLEAR!!";
            AudioManager.i.PlayOneShot(3);
            Debug.Log(0);
        }
        else
        {
            resultText.text = "FAILED";
            AudioManager.i.PlayOneShot(4);
        }
    }

    void OnClickRestartButton()
    {
        AudioManager.i.PlayOneShot(2);
        Variables.screenState = ScreenState.START;

        DOVirtual.DelayedCall(0.4f, () =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
    }
}
