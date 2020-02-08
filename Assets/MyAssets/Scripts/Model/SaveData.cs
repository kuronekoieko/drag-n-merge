using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData
{
    public static SaveData i { get { return _i; } }
    private static SaveData _i = new SaveData();
    public int eraseTargetBlockCount;
    public int sumOfErasedBlockNumbers;
    public int coinCount;
    public int[] itemCounts;
}
