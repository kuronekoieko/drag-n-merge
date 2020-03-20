using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    void Awake()
    {

        // 開発している画面を元に縦横比取得 (縦画面) iPhone6, 6sサイズ
        float developAspect = 750.0f / 1334.0f;
        // 横画面で開発している場合は以下の用に切り替えます
        //float developAspect = 1334.0f / 750.0f;

        // 実機のサイズを取得して、縦横比取得
        float deviceAspect = (float)Screen.width / (float)Screen.height;

        // 実機と開発画面との対比
        float scale = deviceAspect / developAspect;

        Camera mainCamera = Camera.main;

        // カメラに設定していたorthographicSizeを実機との対比でスケール
        float deviceSize = mainCamera.orthographicSize;
        // scaleの逆数
        float deviceScale = 1.0f / scale;
        // orthographicSizeを計算し直す
        mainCamera.orthographicSize = deviceSize * deviceScale;

    }
}
