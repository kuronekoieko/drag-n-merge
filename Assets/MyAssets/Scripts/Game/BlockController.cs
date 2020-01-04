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
    Vector2 wallMaxPos;
    Vector2 wallMinPos;
    GridIndex pointerDownIndex;

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
        this.ObserveEveryValueChanged(num => this.num)
               .Subscribe(num => { SetBlockView(); })
               .AddTo(this.gameObject);

        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        blockState = BlockState.STOP;
        gameObject.SetActive(false);

        //壁
        wallMinPos.x = Variables.blockLowerLeftPos.x;
        wallMaxPos.x = Variables.blockLowerLeftPos.x * -1;
        wallMaxPos.y = Variables.blockLowerLeftPos.y + Values.BROCK_DISTANCE * (Values.BOARD_LENGTH_Y - 2);
        wallMinPos.y = Variables.blockLowerLeftPos.y + Values.BROCK_DISTANCE;
        pointerDownIndex = new GridIndex();
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
        if (underBlock == null)
        {
            blockState = BlockState.FALL;
            return;
        }
        if (underBlock.num == num) { blockState = BlockState.FALL; }

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
        pointerDownIndex.x = indexX;
        pointerDownIndex.y = indexY;
    }


    void OnPointerUp()
    {
        //突き抜け対策
        if (IsOverlapDifferentBlock())
        {
            TransrateBlock(pointerDownIndex.x, pointerDownIndex.y);
        }
        else
        {
            TransrateBlock(indexX, indexY);
        }

        blockState = BlockState.STOP;
        Variables.isDragging = false;
    }



    void OnDrag()
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 maxPos = wallMaxPos;
        Vector2 minPos = wallMinPos;


        BlockController underBlock = GetLimitY(minPos.y, -1, out float minY);
        minPos.y = minY;
        BlockController upperBlock = GetLimitY(maxPos.y, 1, out float maxY);
        maxPos.y = maxY;

        minPos.x = GetLimitX(minPos.x, -1);
        maxPos.x = GetLimitX(maxPos.x, 1);
        worldPos.x = Mathf.Clamp(worldPos.x, minPos.x, maxPos.x);
        worldPos.y = Mathf.Clamp(worldPos.y, minPos.y, maxPos.y);


        worldPos = GetCollisionDiagonalBlockLimit(worldPos, 1, -1);
        worldPos = GetCollisionDiagonalBlockLimit(worldPos, -1, -1);
        // worldPos = GetCollisionDiagonalBlockLimit(worldPos, 1, 1);
        //  worldPos = GetCollisionDiagonalBlockLimit(worldPos, -1, 1);

        //突き抜け対策
        if (IsOverlapDifferentBlock1(worldPos))
        {
            //    return;
        }

        transform.position = worldPos;
    }

    bool IsOverlapDifferentBlock1(Vector2 worldPos)
    {
        int x = Utils.PositionToIndexX(worldPos.x);
        int y = Utils.PositionToIndexY(worldPos.y);
        BlockController block = BlocksManager.i.GetBlock(x, y);
        if (block == null) { return false; }
        if (block.num == num) { return false; }
        if (block.blockState == BlockState.DRAG) { return false; }
        Debug.Log(block.num);
        return true;
    }

    bool IsOverlapDifferentBlock()
    {
        BlockController block = BlocksManager.i.GetBlock(indexX, indexY);
        if (block == null) { return false; }
        if (block.num == num) { return false; }
        if (block.blockState == BlockState.DRAG) { return false; }
        return true;
    }

    BlockController GetLimitY(float wallLimit, int dy, out float limitY)
    {
        BlockController block;
        if (dy == 1)
        {
            block = BlocksManager.i.GetBlock(indexX, indexY + dy);
        }
        else
        {
            block = GetUnderBlock();
        }

        if (block == null)
        {
            limitY = wallLimit;
            return block;
        }
        //同じブロックはマージする
        if (block.num == num)
        {
            limitY = block.transform.position.y;
            return block;
        }
        limitY = block.transform.position.y - (Variables.blockHeight * dy);
        return block;
    }

    float GetLimitX(float wallLimit, int dx)
    {
        BlockController block;
        if (dx == 1)
        {
            block = GetRightBlock();
        }
        else
        {
            block = GetLeftBlock();
        }

        if (block == null) { return wallLimit; }
        //同じブロックはマージする
        if (block.num == num) { return block.transform.position.x; }

        return block.transform.position.x - (Variables.blockHeight * dx);

    }

    Vector2 GetCollisionDiagonalBlockLimit(Vector2 worldPos, int dx, int dy)
    {
        BlockController block = BlocksManager.i.GetBlock(indexX + dx, indexY + dy);
        if (block == null) { return worldPos; }
        if (block.num == num) { return worldPos; }

        float distanceX = Mathf.Abs(worldPos.x - block.transform.position.x);
        float distanceY = Mathf.Abs(worldPos.y - block.transform.position.y);

        if (distanceX > Values.BROCK_DISTANCE / 2) { return worldPos; }
        if (distanceY > Values.BROCK_DISTANCE / 2) { return worldPos; }

        if (distanceX > distanceY)
        {
            worldPos.x = block.transform.position.x - (Variables.blockHeight * dx);
            return worldPos;
        }
        else
        {
            worldPos.y = block.transform.position.y - (Variables.blockHeight * dy);
            return worldPos;
        }

    }

    BlockController GetLeftBlock()
    {
        BlockController dummyBlock = null;
        //上から下へのワープ対策
        for (int ix = indexX - 1; ix >= 0; ix--)
        {
            dummyBlock = BlocksManager.i.GetBlock(ix, indexY);
            if (dummyBlock == null) { continue; }
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
        draggingBlock.gameObject.SetActive(false);

        ClearCheck();

        Variables.sumOfErasedBlockNumbers += num;
        bool isBestScore = (SaveData.i.sumOfErasedBlockNumbers < Variables.sumOfErasedBlockNumbers);
        if (isBestScore)
        {
            SaveData.i.sumOfErasedBlockNumbers = Variables.sumOfErasedBlockNumbers;
            SaveDataManager.i.Save();
        }
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
            Variables.gameState = GameState.IN_PROGRESS_TIMER;
        });
    }

    void FailedCheck()
    {
        if (indexY != Values.BOARD_LENGTH_Y - 1) { return; }
        if (!gameObject.activeSelf) { return; }
        Variables.screenState = ScreenState.RESULT;
    }
}

public class GridIndex
{
    public int x;
    public int y;
}
