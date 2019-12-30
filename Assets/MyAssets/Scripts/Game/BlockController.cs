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
    Rigidbody2D rb;
    BlockState blockState;
    public int indexX
    {
        get
        {
            return Utils.PositionToIndexX(transform.position.x);
        }
    }
    public int indexY
    {
        get
        {
            return Utils.PositionToIndexY(transform.position.y);
        }
    }

    public void OnStart()
    {
        SetEventTriggers();
        num = 1;
        this.ObserveEveryValueChanged(num => this.num)
               .Subscribe(num => { textMesh.text = num.ToString(); })
               .AddTo(this.gameObject);

        rb = GetComponent<Rigidbody2D>();
        blockState = BlockState.STOP;
        gameObject.SetActive(false);
    }

    public void SetNewLine()
    {
        blockState = BlockState.STOP;
        num = 1;
    }

    public void OnUpdate()
    {

        BlockController underBlock = BlocksManager.i.GetBlock(indexX, indexY - 1);
        if (underBlock == null) { blockState = BlockState.FALL; }

        if (blockState == BlockState.FALL)
        {
            FallDown();
        }


    }

    void FallDown()
    {
        transform.Translate(0, -0.1f, 0);
        float bottomY = Variables.blockLowerLeftPos.y + Values.BROCK_DISTANCE;
        if (transform.position.y > bottomY) { return; }
        blockState = BlockState.STOP;
        int bottomIndexY = Utils.PositionToIndexY(bottomY);
        transform.position = Utils.IndexToPosition(indexX, bottomIndexY);
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
        blockState = BlockState.DRAG;
        //Variables.draggingBlockNum = num;
        Variables.isDragging = true;
    }


    void OnPointerUp()
    {
        TransrateBlock(indexX, indexY);
        BlockController block = BlocksManager.i.GetBlock(indexX, indexY - 1);
        if (block)
        {
            blockState = BlockState.STOP;
        }
        else
        {
            blockState = BlockState.FALL;
        }
        Variables.isDragging = false;
    }

    void OnDrag()
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (worldPos.x < Variables.blockLowerLeftPos.x)
        {
            worldPos.x = Variables.blockLowerLeftPos.x;
        }

        float rightX = Variables.blockLowerLeftPos.x * -1;
        if (worldPos.x > rightX)
        {
            worldPos.x = rightX;
        }

        float bottomY = Variables.blockLowerLeftPos.y + Values.BROCK_DISTANCE;
        if (worldPos.y < bottomY)
        {
            worldPos.y = bottomY;
        }

        float topY = Variables.blockLowerLeftPos.y + Values.BROCK_DISTANCE * (Values.BOARD_LENGTH_Y - 2);
        if (worldPos.y > topY)
        {
            worldPos.y = topY;
        }

        worldPos.y = GetCollisionUnderBlockLimit(worldPos.y);
        worldPos.x = GetCollisionLeftBlockLimit(worldPos.x);
        worldPos.x = GetCollisionRightBlockLimit(worldPos.x);

        transform.position = worldPos;
    }

    float GetCollisionUnderBlockLimit(float worldPosY)
    {
        BlockController underBlock = BlocksManager.i.GetBlock(indexX, indexY - 1);
        if (underBlock == null) { return worldPosY; }
        if (underBlock.num == num) { return worldPosY; }
        float fixedY = underBlock.transform.position.y + Variables.blockHeight;
        if (worldPosY > fixedY) { return worldPosY; }
        return fixedY;
    }

    float GetCollisionLeftBlockLimit(float worldPosX)
    {
        int sign = -1;
        BlockController leftBlock = BlocksManager.i.GetBlock(indexX + sign, indexY);
        if (leftBlock == null) { return worldPosX; }
        if (leftBlock.num == num) { return worldPosX; }
        float leftFixedX = leftBlock.transform.position.x - (Variables.blockHeight * sign);
        if (worldPosX > leftFixedX) { return worldPosX; }
        return leftFixedX;
    }

    float GetCollisionRightBlockLimit(float worldPosX)
    {
        int sign = 1;

        BlockController rightBlock = BlocksManager.i.GetBlock(indexX + sign, indexY);
        if (rightBlock == null) { return worldPosX; }
        if (rightBlock.num == num) { return worldPosX; }

        float rightFixedX = rightBlock.transform.position.x - (Variables.blockHeight * sign);
        if (worldPosX < rightFixedX) { return worldPosX; }

        return rightFixedX;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        BlockController block;
        switch (blockState)
        {
            case BlockState.STOP:

                break;
            case BlockState.DRAG:

                break;
            case BlockState.FALL:

                block = col.gameObject.GetComponent<BlockController>();
                if (block.num != num)
                {
                    blockState = BlockState.STOP;
                    TransrateBlock(indexX, block.indexY + 1);
                }
                break;
        }
    }

    void OnCollisionStay2D(Collision2D col)
    {
        BlockController block;
        switch (blockState)
        {
            case BlockState.STOP:
                float distance = (col.transform.position - transform.position).magnitude;
                if (distance > 0.3f) { return; }
                block = col.gameObject.GetComponent<BlockController>();
                if (block.num != num) { return; }
                num++;
                block.gameObject.SetActive(false);

                //Debug.Log("マージ");
                if (num == Variables.targetNum)
                {
                    Variables.screenState = ScreenState.RESULT;
                    Variables.resultState = ResultState.WIN;
                }
                break;
            case BlockState.DRAG:
                break;
            case BlockState.FALL:
                break;
        }
    }

    public void TransrateBlock(int indexX, int indexY)
    {
        transform.position = Utils.IndexToPosition(indexX, indexY);
        if (indexY == Values.BOARD_LENGTH_Y - 1 && gameObject.activeSelf)
        {
            Variables.screenState = ScreenState.RESULT;
            Variables.resultState = ResultState.LOSE;
        }
    }
}
