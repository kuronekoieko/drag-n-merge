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
    [SerializeField] Text highScoreText;
    [SerializeField] Text scoreText;
    [SerializeField] Button nextButton;
    [SerializeField] Text nextButtonText;
    [SerializeField] Button twitterButton;
    [SerializeField] Text shareText;
    //[SerializeField] Text levelText;
    string tweetText;
    bool isUpdateHighScore;
    int highScoreBlockCountBeforeGame;

    public void OnStart()
    {

        this.ObserveEveryValueChanged(screenState => Variables.screenState)
            .Where(screenState => screenState == ScreenState.RESULT)
            .Subscribe(timer => { OnOpen(); })
            .AddTo(this.gameObject);

        nextButton.onClick.AddListener(OnClickRestartButton);
        twitterButton.onClick.AddListener(OnClickTwitterButton);

        Anim();
        TwitterButtonAnim();
    }

    public void OnInitialize()
    {
        gameObject.SetActive(false);
        SetActiveShareGroup(isActive: false);
        //スタート時のハイスコアを結果画面で出す
        highScoreBlockCountBeforeGame = SaveData.i.eraseTargetBlockCount;
        highScoreText.text = "HIGH SCORE : " + highScoreBlockCountBeforeGame;
    }

    void OnOpen()
    {
        gameObject.SetActive(true);
        scoreText.text = "x " + Variables.eraseTargetBlockCount;
        isUpdateHighScore = (highScoreBlockCountBeforeGame < Variables.eraseTargetBlockCount);
        FirebaseAnalyticsManager.i.LogEvent("スコア:" + Variables.eraseTargetBlockCount);
        //クリア音
        //AudioManager.i.PlayOneShot(3);
        SetActiveShareGroup(isActive: true);
        // 警告音
        AudioManager.i.PlayOneShot(4);
        ReviewGuidance();
    }

    void OnClickRestartButton()
    {
        AudioManager.i.PlayOneShot(2);
        Variables.screenState = ScreenState.INITIALIZE;
        gameObject.SetActive(false);
    }

    void SetTweetText()
    {
        string a = "";
        Debug.Log(isUpdateHighScore);
        if (isUpdateHighScore)
        {
            if (Utils.IsLanguageJapanese())
            {
                a = "ハイスコア更新!\n";

            }
            else
            {
                a = "You updated high score!\n";
            }
        }


        if (Utils.IsLanguageJapanese())
        {
            tweetText = a + "あなたのスコアは " + Variables.eraseTargetBlockCount + " でした！";
        }
        else
        {
            tweetText = a + "Your score is ... " + Variables.eraseTargetBlockCount + " !!";
        }
        Debug.Log(tweetText);
    }


    public void OnClickTwitterButton()
    {
        FirebaseAnalyticsManager.i.LogEvent("ツイートボタン");
        SetTweetText();
        //urlの作成
        string esctext = UnityWebRequest.EscapeURL(tweetText + "\n");
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

    void Anim()
    {
        nextButton.transform.DOScale(1.1f, 0.5f)
               .OnComplete(() =>
               {
                   nextButton.transform.DOScale(1f, 0.5f)
                           .OnComplete(() =>
                           {
                               Anim();
                           });
               });
    }


    void TwitterButtonAnim()
    {
        twitterButton.transform.DOScale(1.1f, 0.5f)
               .OnComplete(() =>
               {
                   twitterButton.transform.DOScale(1f, 0.5f)
                           .OnComplete(() =>
                           {
                               TwitterButtonAnim();
                           });
               });
    }
}
