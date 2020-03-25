using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class CSVManagr : MonoBehaviour
{
    [SerializeField] TextAsset masterDataCSV;
    public void OnStart()
    {
        SetMasterData();
        SetAdStages();
    }

    void SetAdStages()
    {
        TextAsset[] adStageCSVs = GetTextAssets("AdStages");

        Variables.stageNums = new List<int[,]>();
        for (int i = 0; i < adStageCSVs.Length; i++)
        {
            List<string[]> strList = CsvToStrList(adStageCSVs[i]);

            int[,] nums = new int[6, 8];
            for (int iy = strList.Count - 1; iy > -1; iy--)
            {
                for (int ix = 0; ix < strList[iy].Length; ix++)
                {
                    if (int.TryParse(strList[iy][ix], out int num))
                    {
                        nums[ix, strList.Count - 1 - iy] = num;
                    }
                }
                Debug.Log(iy);
            }
            Variables.stageNums.Add(nums);
        }
        for (int i = 0; i < 6; i++)
        {
            //  Debug.Log(Variables.stageNums[0][i, 0]);
        }
    }

    TextAsset[] GetTextAssets(string path)
    {
        UnityEngine.Object[] resources = Resources.LoadAll(path);
        TextAsset[] textAssets = new TextAsset[resources.Length];

        for (int i = 0; i < resources.Length; i++)
        {
            textAssets[i] = (TextAsset)resources[i];
        }
        return textAssets;
    }

    void SetMasterData()
    {
        Variables.masterDatas = new List<MasterData>();
        ParseDatas(masterDataCSV, (rowStrs, iy) =>
        {
            MasterData masterData = new MasterData();

            if (int.TryParse(rowStrs[0], out int targetBlockCount))
            {
                masterData.targetBlockCount = targetBlockCount;
            }

            if (float.TryParse(rowStrs[1], out float timeLimit))
            {
                masterData.timeLimit = timeLimit;
            }

            if (int.TryParse(rowStrs[2], out int coinCount))
            {
                masterData.coinCount = coinCount;
            }

            Variables.masterDatas.Add(masterData);
        });
    }

    void ParseDatas(TextAsset csv, Action<string[], int> Action)
    {
        List<string[]> strList = CsvToStrList(csv);
        for (int iy = 1; iy < strList.Count; iy++)
        {
            Action(strList[iy], iy);
        }
    }

    List<string[]> CsvToStrList(TextAsset csvFile)
    {
        var strList = new List<string[]>();
        StringReader reader = new StringReader(csvFile.text);
        while (reader.Peek() != -1) // reader.Peaekが-1になるまで
        {
            string line = reader.ReadLine(); // 一行ずつ読み込み
            strList.Add(line.Split(',')); // , 区切りでリストに追加
        }
        return strList;
    }

}
