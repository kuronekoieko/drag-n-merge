using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyGame/Create StageLevelData", fileName = "StageLevelData")]
public class StageLevelData : ScriptableObject
{
    public StageLevel[] stageLevels;

    private static StageLevelData _i;
    public static StageLevelData i
    {
        get
        {
            string PATH = "ScriptableObjects/" + nameof(StageLevelData);
            //初アクセス時にロードする
            if (_i == null)
            {
                _i = Resources.Load<StageLevelData>(PATH);

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
public class StageLevel
{
    public float timeLimit;
    public int targetNum;
}
