using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetBlockView : MonoBehaviour
{
    [SerializeField] Text numText;
    [SerializeField] Image targetBlockImage;


    void Start()
    {
        numText.text = Values.TARGET_BLOCK_NUM.ToString();
        numText.color = BlockColorData.i.blockColors[Values.TARGET_BLOCK_NUM - 1].textColor;
        targetBlockImage.color = BlockColorData.i.blockColors[Values.TARGET_BLOCK_NUM - 1].color;
    }


}
