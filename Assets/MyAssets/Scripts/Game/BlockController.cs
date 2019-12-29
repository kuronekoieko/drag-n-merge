using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;


/// <summary>
/// Rayを飛ばさずに簡単にオブジェクトのクリックを検知
/// https://nopitech.com/2017/11/25/post-379/
/// 自分用Unityメモ：EventTriggerにスクリプトからEventを追加する
/// http://kasatanet.hatenablog.com/entry/2017/11/19/23342
/// クリックした先のXY平面上の座標を取得する【Unity】
/// https://nyama41.hatenablog.com/entry/click_position_to_xy_plane_position
/// Unity(3D・2D) EventSystemでクリックイベントの制御
/// https://qiita.com/Tachibana446/items/547a299c8829d1177a4d
/// EventTriggerType
/// https://docs.unity3d.com/ja/current/ScriptReference/EventSystems.EventTriggerType.html
/// </summary>
public class BlockController : MonoBehaviour
{
    [SerializeField] TextMesh textMesh;
    EventTrigger eventTrigger;
    int num;
    bool isDrag;
    Rigidbody2D rb;
    int indexX;
    int indexY;

    public void OnStart()
    {
        SetEventTriggers();
        num = 1;
        this.ObserveEveryValueChanged(num => this.num)
               .Subscribe(num => { textMesh.text = num.ToString(); })
               .AddTo(this.gameObject);

        rb = GetComponent<Rigidbody2D>();
        isDrag = false;
        gameObject.SetActive(false);
    }

    public void OnUpdate()
    {
        rb.velocity = Vector2.zero;
    }

    void SetEventTriggers()
    {//コライダーがアタッチされていないと反応しないので注意
        eventTrigger = GetComponent<EventTrigger>();
        eventTrigger.triggers = new List<EventTrigger.Entry>();
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
        isDrag = true;
    }


    void OnPointerUp()
    {
        isDrag = false;
    }

    void OnDrag()
    {
        Vector2 screenPos = Input.mousePosition;
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        transform.position = worldPos;
    }

    void OnCollisionStay2D(Collision2D col)
    {
        float distance = (col.transform.position - transform.position).magnitude;
        if (distance > 0.3f) { return; }
        if (isDrag) { return; }
        BlockController block = col.gameObject.GetComponent<BlockController>();
        if (block.num != num) { return; }
        num++;
        block.gameObject.SetActive(false);
        //Debug.Log("マージ");
        if (num == 5)
        {
            Debug.Log("クリア");
            Variables.screenState = ScreenState.RESULT;
        }
    }
}
