using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class FingerController : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(Debug.isDebugBuild);

        this.ObserveEveryValueChanged(isShowFinger => Variables.isShowFinger)
            .Subscribe(isShowFinger => { gameObject.SetActive(isShowFinger); })
            .AddTo(this.gameObject);
    }


    void Update()
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = worldPos;
    }


}
