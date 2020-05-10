#if UNITY_IPHONE
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;
using System;
using UnityEditor.Build;

public class MyBuildPostprocessor : IPreprocessBuild
{
    // 実行順
    public int callbackOrder { get { return 0; } }


    // ビルド前処理
    public void OnPreprocessBuild(BuildTarget target, string path)
    {
    }

    [PostProcessBuildAttribute(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
    }

    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
        string projectPath = PBXProject.GetPBXProjectPath(path);

        PBXProject pbxProject = new PBXProject();
        pbxProject.ReadFromFile(projectPath);

        string target = pbxProject.TargetGuidByName("Unity-iPhone");


        //pbxProject.AddCapability(target, PBXCapabilityType.InAppPurchase);

        // Plistの設定のための初期化
        var plistPath = Path.Combine(path, "Info.plist");
        var plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        //日付とか
        string dateName = DateTime.Today.Month.ToString("D2") + DateTime.Today.Day.ToString("D2");
        string timeName = DateTime.Now.Hour.ToString("D2") + DateTime.Now.Minute.ToString("D2");

        if (Debug.isDebugBuild)
        {
            //アプリ名
            plist.root.SetString("CFBundleDisplayName", $"{dateName}_debug");

            //bundleId
            pbxProject.SetBuildProperty(target, "PRODUCT_BUNDLE_IDENTIFIER", Application.identifier + ".dev");
        }

        //ipa名
        string buildMode = Debug.isDebugBuild ? "debug" : "release";
        string name = $"{Application.productName}_{buildMode}_ver{Application.version}_{dateName}_{timeName}";
        Debug.Log($"~~~~~~~~~~~~~~~\n{name}\n~~~~~~~~~~~~~~~");

        //tenjinの計測フレームワーク
        //https://github.com/tenjin/tenjin-ios-sdk
        string[] fileNames = {
            "AdSupport.framework",
            "StoreKit.framework",
            "iAd.framework"
        };

        foreach (var fileName in fileNames)
        {
            pbxProject.AddFrameworkToProject(target, fileName, false);
        }

        plist.WriteToFile(plistPath);
        pbxProject.WriteToFile(projectPath);
    }

}
#endif