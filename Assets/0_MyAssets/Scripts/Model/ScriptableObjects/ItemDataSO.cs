using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyGame/Create ItemDataSO", fileName = "ItemDataSO")]
public class ItemDataSO : ScriptableObject
{
    public ItemData[] itemDatas;

    private static ItemDataSO _i;
    public static ItemDataSO i
    {
        get
        {
            string PATH = "ScriptableObjects/" + nameof(ItemDataSO);
            //初アクセス時にロードする
            if (_i == null)
            {
                _i = Resources.Load<ItemDataSO>(PATH);

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
public class ItemData
{
    public int coinCount;
    public string name;
}
