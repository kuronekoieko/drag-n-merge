using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyGame/Create BlockColorData", fileName = "BlockColorData")]
public class BlockColorData : ScriptableObject
{
    public BlockColor[] blockColors;
}

[System.Serializable]
public class BlockColor
{
    public Color color;
    public Color textColor;
    public int num;
}
