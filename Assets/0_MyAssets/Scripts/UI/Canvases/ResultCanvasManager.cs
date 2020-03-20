using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using UnityEngine.Networking;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

/// <summary>
/// 【Unity】Twitterボタン設置とツイートの報酬付与
/// https://qiita.com/Kenji__SHIMIZU/items/d907744a977167d89a78
/// アプリ内でのレビューをUnityで実装(Unity2017.3版)【Unity】【iOS】
/// http://kan-kikuchi.hatenablog.com/entry/iOS_Device_RequestStoreReview
/// Social ConnectorでUnityアプリにソーシャル連携ボタンを追加する
/// https://www.jyuko49.com/entry/2018/04/05/092218
/// </summary>
public class ResultCanvasManager : BaseCanvasManager
{
    [SerializeField] Text bestScoreText;
    [SerializeField] Text scoreText;
    [SerializeField] Text bestTargetCountText;
    [SerializeField] Text targetCountText;
    [SerializeField] Button nextButton;
    [SerializeField] Button twitterButton;
    [SerializeField] Button shareButton;
    [SerializeField] Text shareText;
    [SerializeField] Text coinCountText;
    [SerializeField] Button shopButton;

    public readonly ScreenState thisScreen = ScreenState.RESULT;
    string tweetText;
    bool isUpdateHighScore;
    int highScoreBlockCountBeforeGame;
    int coinCount;

    public override void OnStart()
    {

        base.SetScreenAction(thisScreen: thisScreen);

        nextButton.onClick.AddListener(OnClickRestartButton);
        twitterButton.onClick.AddListener(OnClickTwitterButton);
        shareButton.onClick.AddListener(onClickShare);
        shopButton.onClick.AddListener(OnClickSkinButton);

        Anim(nextButton.transform);
        Anim(twitterButton.transform);
        Anim(shareButton.transform);
        Anim(shopButton.transform);
    }

    public override void OnInitialize()
    {
        gameObject.SetActive(false);
        //スタート時のハイスコアを結果画面で出す
        bestTargetCountText.text = "x " + SaveData.i.eraseTargetBlockCount.ToString();
        bestScoreText.text = SaveData.i.sumOfErasedBlockNumbers.ToString();
    }

    protected override void OnOpen()
    {
        if (Variables.lastScreenState == ScreenState.SKIN) { return; }
        FirebaseAnalyticsManager.i.LogEvent("画面_リザルト");
        gameObject.SetActive(true);
        targetCountText.text = "x " + Variables.eraseTargetBlockCount;
        scoreText.text = Variables.sumOfErasedBlockNumbers.ToString();
        isUpdateHighScore = (highScoreBlockCountBeforeGame < Variables.eraseTargetBlockCount);
        string text = "スコア_" + Variables.eraseTargetBlockCount;
        FirebaseAnalyticsManager.i.LogEvent(text);
        AudioManager.i.PlayOneShot(4);
        if (SaveData.i.launchCount == 1 || SaveData.i.launchCount == 10)
        {
            ReviewGuidance();
        }


        coinCount = 0;
        for (int i = 0; i < Variables.eraseTargetBlockCount; i++)
        {
            coinCount += Utils.GetMasterData(targetBlockCount: i).coinCount;
        }
        SaveData.i.coinCount += coinCount;
        SaveDataManager.i.Save();
        coinCountText.text = "+ " + coinCount;
    }

    protected override void OnClose()
    {
        gameObject.SetActive(Variables.screenState == ScreenState.SKIN);
    }

    void OnClickRestartButton()
    {
        FirebaseAnalyticsManager.i.LogEvent("リスタートボタン");
        AudioManager.i.PlayOneShot(1);
        Variables.screenState = ScreenState.INITIALIZE;
    }

    void OnClickSkinButton()
    {
        FirebaseAnalyticsManager.i.LogEvent("リザルト_ショップボタン");
        AudioManager.i.PlayOneShot(1);
        Variables.screenState = ScreenState.SKIN;
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
             Device.RequestStoreReview();
#elif UNITY_ANDROID
            Application.OpenURL("market://details?id="+Application.productName);
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
