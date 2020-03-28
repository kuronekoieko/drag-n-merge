using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerController : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(Debug.isDebugBuild);
        gameObject.SetActive(false);
    }


    void Update()
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = worldPos;
    }


}
