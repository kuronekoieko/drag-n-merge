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
/// Social ConnectorでUnityアプリにソーシャル連携ボタンを追加する
/// https://www.jyuko49.com/entry/2018/04/05/092218
/// </summary>
public class ResultCanvasManager : MonoBehaviour
{
    [SerializeField] Text bestScoreText;
    [SerializeField] Text scoreText;
    [SerializeField] Text bestTargetCountText;
    [SerializeField] Text targetCountText;
    [SerializeField] Button nextButton;
    [SerializeField] Button twitterButton;
    [SerializeField] Button shareButton;
    [SerializeField] Text shareText;

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
        shareButton.onClick.AddListener(onClickShare);

        Anim(nextButton.transform);
        Anim(twitterButton.transform);
        Anim(shareButton.transform);
    }

    public void OnInitialize()
    {
        gameObject.SetActive(false);
        //スタート時のハイスコアを結果画面で出す
        bestTargetCountText.text = "x " + SaveData.i.eraseTargetBlockCount.ToString();
        bestScoreText.text = SaveData.i.sumOfErasedBlockNumbers.ToString();
    }

    void OnOpen()
    {
        gameObject.SetActive(true);
        targetCountText.text = "x " + Variables.eraseTargetBlockCount;
        scoreText.text = Variables.sumOfErasedBlockNumbers.ToString();
        isUpdateHighScore = (highScoreBlockCountBeforeGame < Variables.eraseTargetBlockCount);
        string text = "スコア:" + Variables.eraseTargetBlockCount;
        FirebaseAnalyticsManager.i.LogEvent(text);
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
        if (isUpdateHighScore)
        {
            if (Utils.IsLanguageJapanese())
            {
                a = "ハイスコア更新!\n";
            }
            else
            {
                a = "You updated best score!\n";
            }
        }


        if (Utils.IsLanguageJapanese())
        {
            tweetText = a + "結果は...\n\n作った10のブロック : " + Variables.eraseTargetBlockCount + " 個\n総得点 : " + Variables.sumOfErasedBlockNumbers + "\n\nでした！\nみんなもやってみよう！\n";
        }
        else
        {
            tweetText = a + "Your score is ... \n\nThe number of 10 blocks : " + Variables.eraseTargetBlockCount + "\nTotal Score : " + Variables.sumOfErasedBlockNumbers + "\n\nLet's play this game!";
        }

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

    void Anim(Transform transform)
    {
        transform.DOScale(1.1f, 0.5f)
               .OnComplete(() =>
               {
                   transform.DOScale(1f, 0.5f)
                            .OnComplete(() =>
                            {
                                Anim(transform);
                            });
               });
    }

    public void onClickShare()
    {
        FirebaseAnalyticsManager.i.LogEvent("ソーシャル連携ボタン");
        StartCoroutine("_share");
    }

    private IEnumerator _share()
    {
        ScreenCapture.CaptureScreenshot(Application.persistentDataPath + "image.png");
        //Application.CaptureScreenshot("image.png");
        yield return null;

        SetTweetText();
        //urlの作成
        string esctext = tweetText + "\n";
        string esctag = "ColorfulMerge\n";
        string hushTag = "#";
        string text = esctext + hushTag + esctag;
        var imagePath = Application.persistentDataPath + "/image.png";
        Debug.Log(imagePath);

#if !UNITY_EDITOR
        SocialConnector.SocialConnector.Share(text, Strings.APP_STORE_URL, imagePath);
#endif
    }
}
