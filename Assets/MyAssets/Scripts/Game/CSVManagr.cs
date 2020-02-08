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
        Variables.masterDatas = new List<MasterData>();
        ParseDatas((rowStrs) =>
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

    void ParseDatas(Action<string[]> Action)
    {
        List<string[]> strList = CsvToStrList(masterDataCSV);
        for (int iy = 1; iy < strList.Count; iy++)
        {
            Action(strList[iy]);
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
