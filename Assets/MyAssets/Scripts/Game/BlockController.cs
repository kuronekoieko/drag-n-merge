using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Rayを飛ばさずに簡単にオブジェクトのクリックを検知
/// https://nopitech.com/2017/11/25/post-379/
/// 自分用Unityメモ：EventTriggerにスクリプトからEventを追加する
/// http://kasatanet.hatenablog.com/entry/2017/11/19/23342
/// クリックした先のXY平面上の座標を取得する【Unity】
/// https://nyama41.hatenablog.com/entry/click_position_to_xy_plane_position
/// Unity(3D・2D) EventSystemでクリックイベントの制御
/// https://qiita.com/Tachibana446/items/547a299c8829d1177a4d
/// </summary>
public class BlockController : MonoBehaviour
{

    EventTrigger eventTrigger;

    public void OnStart()
    {
        SetEventTriggers();
    }

    public void OnUpdate()
    {
    }

    void SetEventTriggers()
    {//コライダーがアタッチされていないと反応しないので注意
        eventTrigger = GetComponent<EventTrigger>();
        eventTrigger.triggers = new List<EventTrigger.Entry>();
        EventTrigger.Entry[] entries = new EventTrigger.Entry[2];

        for (int i = 0; i < entries.Length; i++)
        {
            entries[i] = new EventTrigger.Entry();
        }

        entries[0].eventID = EventTriggerType.PointerDown;
        entries[0].callback.AddListener((x) => OnPointerDown());
        entries[1].eventID = EventTriggerType.PointerUp;
        entries[1].callback.AddListener((x) => OnPointerUp());

        eventTrigger.triggers.Add(entries[0]);
        eventTrigger.triggers.Add(entries[1]);
        Debug.Log(entries[0].callback);
    }

    void OnPointerDown()
    {
        Debug.Log("down");
    }


    void OnPointerUp()
    {
        Debug.Log("up");
    }
}
