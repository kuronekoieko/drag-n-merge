using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyGame/Create BackgroundDataSO", fileName = "BackgroundDataSO")]
public class BackgroundDataSO : ScriptableObject
{
    public BackgroundData[] backgroundDatas;

    private static BackgroundDataSO _i;
    public static BackgroundDataSO i
    {
        get
        {
            string PATH = "ScriptableObjects/" + nameof(BackgroundDataSO);
            //初アクセス時にロードする
            if (_i == null)
            {
                _i = Resources.Load<BackgroundDataSO>(PATH);

                //ロード出来なかった場合はエラーログを表示
                if (_i == null)
                {
                    Debug.LogError(PATH + " not found");
                }
            }

            return _i;
        }
    }
}

[System.Serializable]
public class BackgroundData
{
    public Sprite sprite;
    public bool isTextBlack;
    public int coinCount;
}
