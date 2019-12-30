using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.SceneManagement;

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
        resultText.text = Variables.resultState == ResultState.WIN ? "YOU WIN" : "YOU LOSE";
    }

    void OnClickRestartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
