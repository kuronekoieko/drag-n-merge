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
/// ParticleのstartColorをスクリプトで変更する
/// http://am1tanaka.hatenablog.com/entry/2017/07/01/183619
/// </summary>
public class BlockController : MonoBehaviour
{
    [SerializeField] TextMesh textMesh;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] ParticleSystem hitPS;
    EventTrigger eventTrigger;
    public int num
    {
        set { _num = (value < 1) ? 1 : value; }
        get { return _num; }
    }
    private int _num;
    Rigidbody2D rb;
    BoxCollider2D boxCollider;
    public BlockState blockState { get; private set; }
    BlockType blockType;
    Vector2 wallMaxPos;
    Vector2 wallMinPos;
    GridIndex pointerDownIndex;
    float blockHeight;

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

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        pointerDownIndex = new GridIndex();
        eventTrigger = GetComponent<EventTrigger>();
        eventTrigger.triggers = new List<EventTrigger.Entry>();
    }

    public void OnStart(string name)
    {
        this.name = name;
        num = 1;
        blockHeight = boxCollider.size.x;
        SetEventTriggers();
        this.ObserveEveryValueChanged(num => this._num)
               .Subscribe(num => { SetBlockView(); })
               .AddTo(this.gameObject);
        blockState = BlockState.STOP;
        blockType = BlockType.NUMBER;

        gameObject.SetActive(false);

        //壁
        wallMinPos.x = Values.BLOCK_POS_LOWER_LEFT.x;
        wallMaxPos.x = Values.BLOCK_POS_LOWER_LEFT.x * -1;
        wallMaxPos.y = Values.BLOCK_POS_LOWER_LEFT.y + Values.BROCK_DISTANCE * (Values.BOARD_LENGTH_Y - 2);
        wallMinPos.y = Values.BLOCK_POS_LOWER_LEFT.y + Values.BROCK_DISTANCE;
    }

    public void SetNewLine()
    {
        blockState = BlockState.STOP;
        blockType = BlockType.NUMBER;
        boxCollider.enabled = true;
    }

    public void SetRandomNum()
    {
        int max = Values.RANDOM_RANGE_MAX;
        num = Random.Range(1, max);
        BlockController upperBlock = BlocksManager.i.GetBlock(indexX, 1);
        if (upperBlock == null) { return; }

        int c = 0;
        while (upperBlock.num == num)
        {
            num = Random.Range(1, max);
            c++;
            if (c == 50) break;
        }
        return;
        blockType = GetBlockType();
        if (blockType == BlockType.NUMBER) { return; }
        int tmpNum = (int)blockType;
        if (upperBlock.num == tmpNum) { return; }
        num = tmpNum;
    }

    BlockType GetBlockType()
    {
        int specialBlockProbability = Random.Range(1, 101);
        if (specialBlockProbability < 10) { return BlockType.FALL_LINE; }
        if (specialBlockProbability < 20) { return BlockType.CHANGE_NUMBER_COLUMN; }
        return BlockType.NUMBER;
    }

    public void SetSameNumAsUnderBlock()
    {
        BlockController underBlock = GetUnderBlock();
        num = 1;
        if (underBlock == null) { return; }
        if (underBlock.num > Values.TARGET_BLOCK_NUM) { return; }
        num = underBlock.num;

    }

    public void OnUpdate()
    {

        FallCheckOnUpdate();

        if (blockState == BlockState.FALL)
        {
            FallDown();
            StopFall();
        }
    }

    void StopFall()
    {
        BlockController underBlock = BlocksManager.i.GetBlock(indexX: indexX, indexY: indexY - 1);
        if (underBlock == null) { return; }

        float limitY = underBlock.transform.position.y + Values.BROCK_DISTANCE;
        if (transform.position.y > limitY) { return; }
        if (underBlock.num == num) { return; }

        TransrateBlock(indexX, underBlock.indexY + 1);
        blockState = BlockState.STOP;
    }

    void SetBlockView()
    {
        textMesh.color = BlockColorData.i.blockColors[num - 1].textColor;
        spriteRenderer.color = BlockColorData.i.blockColors[num - 1].color;
        ParticleSystem.MainModule par = hitPS.main;
        par.startColor = BlockColorData.i.blockColors[num - 1].color;

        string text = num.ToString();
        if (num <= Values.TARGET_BLOCK_NUM)
        {
            blockType = BlockType.NUMBER;
        }

        Dictionary<BlockType, string> t = new Dictionary<BlockType, string>()
        {
            {BlockType.ADD_NUMBER_ALL,"+1"},
            {BlockType.JOKER,"J"},
            {BlockType.FALL_LINE,"F"},
            {BlockType.CHANGE_NUMBER_COLUMN,"C"}
        };

        if (t.TryGetValue(blockType, out string val))
        {
            text = val;
        }

        textMesh.text = text;
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
        float bottomY = Values.BLOCK_POS_LOWER_LEFT.y + Values.BROCK_DISTANCE;
        if (transform.position.y > bottomY) { return; }
        blockState = BlockState.STOP;
        int bottomIndexY = Utils.PositionToIndexY(bottomY);
        transform.position = Utils.IndexToPosition(indexX, bottomIndexY);
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
        blockState = BlockState.DRAG;
        Variables.isDragging = true;
        pointerDownIndex.x = indexX;
        pointerDownIndex.y = indexY;
        Variables.comboCount = 0;
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

        minPos.y = GetLimit(minPos, 0, -1).y;
        maxPos.y = GetLimit(maxPos, 0, 1).y;
        minPos.x = GetLimit(minPos, -1, 0).x;
        maxPos.x = GetLimit(maxPos, 1, 0).x;

        worldPos.x = Mathf.Clamp(worldPos.x, minPos.x, maxPos.x);
        worldPos.y = Mathf.Clamp(worldPos.y, minPos.y, maxPos.y);

        worldPos = GetCollisionDiagonalBlockLimit(worldPos, 1, -1);
        worldPos = GetCollisionDiagonalBlockLimit(worldPos, -1, -1);
        worldPos = GetCollisionDiagonalBlockLimit(worldPos, 1, 1);
        worldPos = GetCollisionDiagonalBlockLimit(worldPos, -1, 1);

        transform.position = worldPos;
    }

    bool IsOverlapDifferentBlock()
    {
        BlockController block = BlocksManager.i.GetBlock(indexX, indexY);
        if (block == null) { return false; }
        if (block.num == num) { return false; }
        if (block.blockState == BlockState.DRAG) { return false; }
        return true;
    }

    float GetLimitY(float wallLimit, int dy)
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
            return wallLimit;
        }
        //同じブロックはマージする
        if (block.num == num)
        {
            return block.transform.position.y;
        }
        return block.transform.position.y - (blockHeight * dy);
    }

    Vector2 GetLimit(Vector2 limitPos, int dx, int dy)
    {
        BlockController block;
        if (dx == 1)
        {
            block = GetRightBlock();
        }
        else if (dx == -1)
        {
            block = GetLeftBlock();
        }
        else if (dy == 1)
        {
            block = BlocksManager.i.GetBlock(indexX, indexY + dy);
        }
        else
        {
            block = GetUnderBlock();
        }

        if (block == null) { return limitPos; }
        //同じブロックはマージする
        if (block.num == num) { return block.transform.position; }
        float limitX = block.transform.position.x - (blockHeight * dx);
        float limitY = block.transform.position.y - (blockHeight * dy);

        if (dx == 0) { limitPos.y = limitY; }
        if (dy == 0) { limitPos.x = limitX; }

        return limitPos;

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
            worldPos.x = block.transform.position.x - (blockHeight * dx);
            return worldPos;
        }
        else
        {
            worldPos.y = block.transform.position.y - (blockHeight * dy);
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

        if (num < Values.TARGET_BLOCK_NUM) { num++; }
        hitPS.Play();
        SaveBestScore();
        ScoreNumTextManager.i.ShowScoreNum(
            scoreNum: num,
            pos: transform.position);

        //タイマーが止まるため
        Variables.isDragging = false;
        draggingBlock.gameObject.SetActive(false);
        Variables.comboCount++;

        if (num == Values.TARGET_BLOCK_NUM)
        {   //クリア音
            AudioManager.i.PlayOneShot(3);
            SaveBestTargetBlockCount();
            MoveFadeOut();
            return;
        }

        if (num > Values.TARGET_BLOCK_NUM)
        {
            AudioManager.i.PlayOneShot(3);
            MoveFadeOut();
            if (num == 11)
            {
                BlocksManager.i.ShowBlocksTopLine();
            }
            if (num == 12)
            {
                BlocksManager.i.MakeNumbersConsecutive(indexX);
            }

            return;
        }

        //マージ音
        AudioManager.i.PlayOneShot(0);
        MergeAnim();
    }

    void MergeAnim()
    {
        Sequence sequence0 = DOTween.Sequence()
            .Append(transform.DOScale(Vector3.one * 1.4f, 0.05f))
            .Append(transform.DOScale(Vector3.one * 1.0f, 0.2f));

        //sequence0.Play();


        Sequence sequence1 = DOTween.Sequence()
            .Append(transform.DOScale(Vector3.one * 1.2f, 0f))
            .Append(transform.DOScale(Vector3.one * 0.7f, 0.05f))
            .Append(transform.DOScale(Vector3.one * 1.0f, 0.05f));


        sequence1.Play();
    }

    void SaveBestScore()
    {
        Variables.sumOfErasedBlockNumbers += num;
        bool isBestScore = (SaveData.i.sumOfErasedBlockNumbers < Variables.sumOfErasedBlockNumbers);
        if (!isBestScore) { return; }
        SaveData.i.sumOfErasedBlockNumbers = Variables.sumOfErasedBlockNumbers;
        SaveDataManager.i.Save();
    }



    void SaveBestTargetBlockCount()
    {
        Variables.eraseTargetBlockCount++;
        bool isBest = SaveData.i.eraseTargetBlockCount < Variables.eraseTargetBlockCount;
        if (!isBest) { return; }
        SaveData.i.eraseTargetBlockCount = Variables.eraseTargetBlockCount;
        SaveDataManager.i.Save();
    }

    void MoveFadeOut()
    {
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
