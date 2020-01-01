using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using UnityEngine.Networking;
using UnityEngine.iOS;

/// <summary>
/// 【Unity】Twitterボタン設置とツイートの報酬付与
/// https://qiita.com/Kenji__SHIMIZU/items/d907744a977167d89a78
/// アプリ内でのレビューをUnityで実装(Unity2017.3版)【Unity】【iOS】
/// http://kan-kikuchi.hatenablog.com/entry/iOS_Device_RequestStoreReview
/// </summary>
public class ResultCanvasManager : MonoBehaviour
{
    [SerializeField] Text resultText;
    [SerializeField] Button restartButton;
    [SerializeField] Button twitterButton;
    [SerializeField] Text shareText;
    public void OnStart()
    {

        this.ObserveEveryValueChanged(screenState => Variables.screenState)
            .Where(screenState => screenState == ScreenState.RESULT)
            .Subscribe(timer => { OnOpen(); })
            .AddTo(this.gameObject);

        restartButton.onClick.AddListener(OnClickRestartButton);
        twitterButton.onClick.AddListener(OnClickTwitterButton);
    }

    public void OnInitialize()
    {
        gameObject.SetActive(false);
        SetActiveShareGroup(isActive: false);
    }

    void OnOpen()
    {
        gameObject.SetActive(true);
        resultText.text = Variables.resultState == ResultState.WIN ? "CLEAR!!" : "FAILED";
        if (Variables.resultState == ResultState.WIN)
        {
            resultText.text = "CLEAR!!";
            AudioManager.i.PlayOneShot(3);
            ReviewGuidance();
            SetActiveShareGroup(isActive: true);
            SaveData.i.clearedLevel++;
            SaveDataManager.i.Save();
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
        Variables.screenState = ScreenState.INITIALIZE;
        gameObject.SetActive(false);

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
        string esctext = UnityWebRequest.EscapeURL(text + "\n");
        string esctag = UnityWebRequest.EscapeURL("ColorfulMerge\n");
        string hushTag = "&hashtags=" + esctag;

        string url = "https://twitter.com/intent/tweet?text=" + esctext + hushTag + Strings.APP_STORE_URL;

        //Twitter投稿画面の起動
        Application.OpenURL(url);
        //Debug.Log("ツイッター");
    }

    void SetActiveShareGroup(bool isActive)
    {
        twitterButton.gameObject.SetActive(isActive);
        shareText.gameObject.SetActive(isActive);
    }

    void ReviewGuidance()
    {
#if UNITY_EDITOR
        //Debug.Log("レビュー誘導表示");
#elif UNITY_IOS
            //IOSReviewRequest.RequestReview();
             Device.RequestStoreReview();
#elif UNITY_ANDROID
           // Application.OpenURL("market://details?id=com.brick.games");
#endif

    }
}
