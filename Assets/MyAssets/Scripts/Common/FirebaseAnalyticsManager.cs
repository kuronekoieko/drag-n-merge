using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unity 用 Google アナリティクスを使ってみる
/// https://firebase.google.com/docs/analytics/unity/start?hl=ja
/// </summary>
public class FirebaseAnalyticsManager
{
    public static FirebaseAnalyticsManager i { get { return _i; } }
    private static FirebaseAnalyticsManager _i = new FirebaseAnalyticsManager();

    public void LogEvent(string parameterValue)
    {
        // Log an event with a string parameter.
        Firebase.Analytics.FirebaseAnalytics
          .LogEvent(
            parameterValue,
            parameterValue,
            parameterValue
          );
    }

    public void LogScreen(string title)
    {
        Firebase.Analytics.FirebaseAnalytics.SetCurrentScreen(
          screenName: title,
          screenClass: title);
    }
}
