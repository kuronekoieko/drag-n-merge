using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FingerController : MonoBehaviour
{
    EventTrigger eventTrigger;
    void Start()
    {
        gameObject.SetActive(Debug.isDebugBuild);
    }


    void Update()
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = worldPos;
    }

    void SetEventTriggers()
    {//コライダーがアタッチされていないと反応しないので注意
        EventTrigger.Entry[] entries = new EventTrigger.Entry[3];

        for (int i = 0; i < entries.Length; i++)
        {
            entries[i] = new EventTrigger.Entry();
        }

        entries[0].eventID = EventTriggerType.PointerDown;
        entries[0].callback.AddListener((x) => OnPointerDown());
        entries[1].eventID = EventTriggerType.PointerUp;
        entries[1].callback.AddListener((x) => OnPointerUp());
        entries[2].eventID = EventTriggerType.Drag;
        entries[2].callback.AddListener((x) => OnDrag());

        for (int i = 0; i < entries.Length; i++)
        {
            eventTrigger.triggers.Add(entries[i]);
        }
    }

    void OnPointerDown()
    {

    }

    void OnPointerUp()
    {

    }

    void OnDrag()
    {

    }

}
