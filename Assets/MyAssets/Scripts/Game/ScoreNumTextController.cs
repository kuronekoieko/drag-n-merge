using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class ScoreNumTextController : MonoBehaviour
{
    TextMeshPro scoreNumTMP;
    Color defaultColor;
    public int indexX { get; private set; }
    public int indexY { get; private set; }
    public bool isWaiting { get; private set; }

    void Awake()
    {
        scoreNumTMP = GetComponent<TextMeshPro>();
    }

    public void OnStart(string name, int indexX, int indexY)
    {
        this.name = name;
        this.indexX = indexX;
        this.indexY = indexY;

        defaultColor = scoreNumTMP.color;
        scoreNumTMP.alpha = 0;
        isWaiting = true;
    }

    public void ShowScoreNum(int scoreNum, Vector3 pos)
    {
        transform.position = pos;
        scoreNumTMP.text = "+" + scoreNum;
        Anim();
    }

    void Anim()
    {
        isWaiting = false;
        scoreNumTMP.color = defaultColor;
        float duration = 2f;
        Vector2 pos = (Vector2)transform.position;
        Vector3[] path = new Vector3[]{
                pos+new Vector2(-0.5f,0.5f),
                pos+new Vector2(0.5f,1f),
                pos+new Vector2(0,1.5f),
        };

        transform
            .DOLocalPath(path, duration, PathType.CatmullRom)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                isWaiting = true;
            });

        DOTween.ToAlpha(
            () => scoreNumTMP.color,
            color => scoreNumTMP.color = color,
            0f, // 目標値
            duration // 所要時間
        );
    }


}
