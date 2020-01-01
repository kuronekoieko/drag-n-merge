using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.Networking;

/// <summary>
/// 【Unity】Twitterボタン設置とツイートの報酬付与
/// https://qiita.com/Kenji__SHIMIZU/items/d907744a977167d89a78
/// </summary>
public class ResultCanvasManager : MonoBehaviour
{
    [SerializeField] Text resultText;
    [SerializeField] Button restartButton;
    [SerializeField] Button twitterButton;
    public void OnStart()
    {

        this.ObserveEveryValueChanged(screenState => Variables.screenState)
            .Where(screenState => screenState == ScreenState.RESULT)
            .Subscribe(timer => { OnOpen(); })
            .AddTo(this.gameObject);

        restartButton.onClick.AddListener(OnClickRestartButton);
        twitterButton.onClick.AddListener(OnClickTwitterButton);

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

    public void OnClickTwitterButton()
    {
        string text = "";
        if (Utils.IsLanguageJapanese())
        {
            text = "「ColorfulMerge」をクリアしました！";
        }
        else
        {
            text = "You have completed this game!「ColorfulMerge」";
        }

        //urlの作成
        string esctext = UnityWebRequest.EscapeURL(text + "\n" + Strings.APP_STORE_URL);
        string esctag = UnityWebRequest.EscapeURL("ColorfulMerge");
        string url = "https://twitter.com/intent/tweet?text=" + esctext + "\n" + "&hashtags=" + esctag;

        //Twitter投稿画面の起動
        Application.OpenURL(url);
    }
}
