using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;
using DG.Tweening;

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
    [SerializeField] SpriteRenderer spriteRenderer;
    EventTrigger eventTrigger;
    public int num { get; private set; }
    Rigidbody2D rb;
    BoxCollider2D boxCollider;
    public BlockState blockState { get; private set; }
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

    int pointerDownIndexX;
    int pointerDownIndexY;

    public void OnStart()
    {
        SetEventTriggers();
        this.ObserveEveryValueChanged(num => this.num)
               .Subscribe(num => { SetBlockView(); })
               .AddTo(this.gameObject);

        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        blockState = BlockState.STOP;
        gameObject.SetActive(false);
    }

    public void SetNewLine()
    {
        blockState = BlockState.STOP;
        int max = Values.RANDOM_RANGE_MAX;
        num = Random.Range(1, max);
        BlockController upperBlock = BlocksManager.i.GetBlock(indexX, 1);
        int c = 0;
        if (upperBlock)
        {
            while (upperBlock.num == num)
            {
                num = Random.Range(1, max);
                c++;
                if (c == 50) break;
            }
        }
        boxCollider.enabled = true;
    }

    public void OnUpdate()
    {

        FallCheckOnUpdate();

        if (blockState == BlockState.FALL)
        {
            FallDown();
        }
    }

    void SetBlockView()
    {

        if (num > 0)
        {
            textMesh.color = BlockColorData.i.blockColors[num - 1].textColor;
            spriteRenderer.color = BlockColorData.i.blockColors[num - 1].color;
        }

        textMesh.text = num.ToString();
    }

    void FallCheckOnUpdate()
    {
        //一列目でバグるため
        if (indexY <= 1) { return; }
        //ドラッグ中のブロックが落ちるため
        if (blockState != BlockState.STOP) { return; }
        BlockController underBlock = BlocksManager.i.GetBlock(indexX, indexY - 1);
        if (underBlock == null) { blockState = BlockState.FALL; }
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
        Variables.isDragging = true;
        pointerDownIndexX = indexX;
        pointerDownIndexY = indexY;
    }


    void OnPointerUp()
    {
        TransrateBlock(indexX, indexY);
        FallCheckOnPointerUp();
        Variables.isDragging = false;
    }

    void FallCheckOnPointerUp()
    {
        BlockController underBlock = BlocksManager.i.GetBlock(indexX, indexY - 1);
        blockState = BlockState.FALL;
        if (underBlock == null) { return; }
        if (underBlock.num == num) { return; }
        blockState = BlockState.STOP;
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

        // worldPos.y = GetCollisionUnderBlockLimit(worldPos.y, dx: 0, dy: -1);
        worldPos = GetAdjacentBlockLimit(worldPos, dx: 0, dy: -1);
        worldPos = GetAdjacentBlockLimit(worldPos, dx: 0, dy: 1);
        worldPos = GetAdjacentBlockLimit(worldPos, dx: -1, dy: 0);
        worldPos = GetAdjacentBlockLimit(worldPos, dx: 1, dy: 0);
        // worldPos = GetAdjacentBlockLimit(worldPos, dx: -1, dy: -1);
        // worldPos = GetAdjacentBlockLimit(worldPos, dx: 1, dy: -1);

        worldPos = GetCollisionDiagonalBlockLimit(worldPos, -1, -1);
        worldPos = GetCollisionDiagonalBlockLimit(worldPos, 1, -1);
        transform.position = worldPos;
    }

    Vector2 GetCollisionDiagonalBlockLimit(Vector2 worldPos, int dx, int dy)
    {
        Vector3 pos = worldPos;
        if (!existAdjacentBlock(dx, dy, out Vector2 limitPos)) { return worldPos; }

        if ((transform.position.x - limitPos.x) * dx > 0 && worldPos.y < limitPos.y)
        {
            pos.y = limitPos.y;
        }


        if (transform.position.y < limitPos.y && (worldPos.x - limitPos.x) * dx >= 0)
        {
            pos.x = limitPos.x;
        }


        return pos;

    }

    Vector2 GetAdjacentBlockLimit(Vector2 worldPos, int dx, int dy)
    {
        if (!existAdjacentBlock(dx, dy, out Vector2 limitPos)) { return worldPos; }

        bool isLimitX = (worldPos.x - limitPos.x) * dx > 0;
        if (isLimitX)
        {
            worldPos.x = limitPos.x;
        }

        bool isLimitY = (worldPos.y - limitPos.y) * dy > 0;
        if (isLimitY)
        {
            worldPos.y = limitPos.y;
        }

        return worldPos;
    }

    bool existAdjacentBlock(int dx, int dy, out Vector2 limitPos)
    {
        limitPos = Vector2.zero;
        BlockController block = BlocksManager.i.GetBlock(indexX + dx, indexY + dy);

        if (dy == -1 && dx == 0)
        {
            block = GetUnderBlock();
        }
        if (dx == -1 && dy == 0)
        {
            block = GetLeftBlock();
        }
        if (dx == 1 && dy == 0)
        {
            block = GetRightBlock();
        }

        if (block == null) { return false; }
        if (block.num == num) { return false; }

        limitPos.x = block.transform.position.x - (Variables.blockHeight * dx);
        limitPos.y = block.transform.position.y - (Variables.blockHeight * dy);
        return true;
    }

    BlockController GetLeftBlock()
    {
        BlockController dummyBlock = null;
        //上から下へのワープ対策
        for (int ix = indexX - 1; ix >= 0; ix--)
        {
            dummyBlock = BlocksManager.i.GetBlock(ix, indexY);
            if (dummyBlock == null) { continue; }
            if (dummyBlock.num == num) { continue; }
            return dummyBlock;
        }
        return null;
    }

    BlockController GetRightBlock()
    {
        BlockController dummyBlock = null;
        //上から下へのワープ対策
        for (int ix = indexX + 1; ix <= Values.BOARD_LENGTH_X; ix++)
        {
            dummyBlock = BlocksManager.i.GetBlock(ix, indexY);
            if (dummyBlock == null) { continue; }
            if (dummyBlock.num == num) { continue; }
            return dummyBlock;
        }
        return null;

    }

    BlockController GetUnderBlock()
    {
        BlockController dummyBlock = null;
        //上から下へのワープ対策
        for (int iy = indexY - 1; iy > 0; iy--)
        {
            dummyBlock = BlocksManager.i.GetBlock(indexX, iy);
            if (dummyBlock == null) { continue; }
            if (dummyBlock.num == num) { continue; }
            return dummyBlock;
        }

        return null;
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
        switch (blockState)
        {
            case BlockState.STOP:
                BlockController draggingBlock = col.gameObject.GetComponent<BlockController>();
                Merge(draggingBlock);
                break;
            case BlockState.DRAG:
                break;
            case BlockState.FALL:
                break;
        }
    }

    void Merge(BlockController draggingBlock)
    {
        float distance = (draggingBlock.transform.position - transform.position).magnitude;
        if (distance > 0.3f) { return; }
        if (draggingBlock.num != num) { return; }
        num++;

        //タイマーが止まるため
        Variables.isDragging = false;
        FallCheckOnMerge();
        FallCheckUpperBlockOnMerge();
        draggingBlock.gameObject.SetActive(false);

        ClearCheck();


    }

    void FallCheckOnMerge()
    {
        BlockController underBlock = BlocksManager.i.GetBlock(indexX, indexY - 1);
        if (underBlock == null) { return; }
        if (underBlock.num != num) { return; }
        blockState = BlockState.FALL;
    }

    void FallCheckUpperBlockOnMerge()
    {
        BlockController upperBlock = BlocksManager.i.GetBlock(indexX, indexY + 1);
        if (upperBlock == null) { return; }
        if (upperBlock.num != num) { return; }
        upperBlock.blockState = BlockState.FALL;
    }

    void ClearCheck()
    {
        if (num != Values.TARGET_BLOCK_NUM)
        {
            AudioManager.i.PlayOneShot(0);
            return;
        }

        Variables.eraseTargetBlockCount++;
        if (SaveData.i.eraseTargetBlockCount < Variables.eraseTargetBlockCount)
        {
            SaveData.i.eraseTargetBlockCount = Variables.eraseTargetBlockCount;
            SaveDataManager.i.Save();
        }
        MoveFadeOut();
    }

    void MoveFadeOut()
    {
        //クリア音
        AudioManager.i.PlayOneShot(3);

        transform.DOScale(Vector3.zero, 0.5f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                boxCollider.enabled = false;
                gameObject.SetActive(false);
                transform.localScale = Vector3.one;
            });
    }

    public void TransrateBlock(int indexX, int indexY)
    {
        transform.position = Utils.IndexToPosition(indexX, indexY);
    }

    public void MoveUpAnim()
    {
        eventTrigger.enabled = false;
        float y = Utils.IndexToPosition(indexX, indexY + 1).y;
        transform.DOMoveY(y, 0.5f)
        .OnComplete(() =>
        {
            FailedCheck();
            eventTrigger.enabled = true;
        });
    }

    void FailedCheck()
    {
        if (indexY != Values.BOARD_LENGTH_Y - 1) { return; }
        if (!gameObject.activeSelf) { return; }
        Variables.screenState = ScreenState.RESULT;
    }
}
