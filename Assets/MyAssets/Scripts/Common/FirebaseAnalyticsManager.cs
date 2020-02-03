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
    bool isAvailable;

    public void OnStart()
    {

#if UNITY_IOS
        isAvailable = true;
#elif UNITY_ANDROID
        isAvailable = false;
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                //   app = Firebase.FirebaseApp.DefaultInstance;
                isAvailable = true;
                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
#endif
    }

    public void LogEvent(string parameterValue)
    {
        if (!isAvailable) { return; }
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
        if (!isAvailable) { return; }
        Firebase.Analytics.FirebaseAnalytics
          .SetCurrentScreen(
            screenName: title,
            screenClass: title
            );
    }
}
