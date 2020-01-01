using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class SaveDataManager : MonoBehaviour
{
    [SerializeField] TextAsset defaultSaveDataJson;
    public static SaveDataManager i;


    public void OnStart()
    {
        i = this;
        LoadUserData();
        /*
        this.ObserveEveryValueChanged(i => SaveData.i)
          .Subscribe(i => { Save(); })
          .AddTo(this.gameObject);
        */
    }

    public void Save()
    {
        //ユーザーデータオブジェクトからjson形式のstringを取得
        string jsonStr = JsonUtility.ToJson(SaveData.i);

        //jsonデータをセットする
        PlayerPrefs.SetString(Strings.KEY_SAVE_DATA, jsonStr);
        //保存する
        PlayerPrefs.Save();
        //Debug.Log("ユーザーデータ保存");
    }

    void LoadUserData()
    {
        //初回起動時のユーザーデータ作成
        string defaultJsonStr = GetDefaultSaveDataStr();
        //PlayerPrefsに保存済みのユーザーデータのstringを取得
        //第二引数に初回起動時のデータを入れる
        string jsonStr = PlayerPrefs.GetString(Strings.KEY_SAVE_DATA, defaultJsonStr);
        //ユーザーデータオブジェクトに読み出したデータを格納
        //※このとき、新しく追加された変数は消されずマージされる
        JsonUtility.FromJsonOverwrite(jsonStr, SaveData.i);
        //アプデ対応
        //AddUserDataInstance();
        //ユーザーデータ保存
        Save();
    }

    string GetDefaultSaveDataStr()
    {
        //ユーザーデータオブジェクトにデフォルトのjsonを書き込む
        //※アップデートで変数の種類が増えたときに初期値を入れておくため
        JsonUtility.FromJsonOverwrite(
            json: defaultSaveDataJson.text,
            objectToOverwrite: SaveData.i);

        //デフォルトのユーザーデータを作成
        string defaultJsonStr = JsonUtility.ToJson(SaveData.i);
        return defaultJsonStr;
    }
}
