﻿using System.Collections;
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
    public int launchCount;
    public List<PossessedBackground> possessedBackgrounds;
}

[System.Serializable]
public class PossessedBackground
{
    public bool isSelected;
    public BGToggleState bGToggleState;
}

[System.Serializable]
public enum BGToggleState
{
    Unlocked = 0,
    ShownLock = 1,
    HiddenLock = 2,
}