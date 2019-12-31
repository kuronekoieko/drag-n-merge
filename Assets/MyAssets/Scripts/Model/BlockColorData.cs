using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyGame/Create BlockColorData", fileName = "BlockColorData")]
public class BlockColorData : ScriptableObject
{
    public BlockColor[] blockColors;

    private static BlockColorData _i;
    public static BlockColorData i
    {
        get
        {
            string PATH = "ScriptableObjects/" + nameof(BlockColorData);
            //初アクセス時にロードする
            if (_i == null)
            {
                _i = Resources.Load<BlockColorData>(PATH);

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
public class BlockColor
{
    public Color color;
    public Color textColor;
    public int num;
}
