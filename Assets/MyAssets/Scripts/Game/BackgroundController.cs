using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    readonly Vector2 defaultSize = new Vector2(5.62f, 6.50f);

    public void OnStart()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetBackground()
    {

        int index = SaveData.i.possessedBackgrounds.FindIndex(p => p.isSelected);
        if (index == -1) { return; }
        spriteRenderer.sprite = BackgroundDataSO.i.backgroundDatas[index].sprite;
        Vector2 size = spriteRenderer.bounds.size;
        transform.localScale = Vector3.one;
        float scale = 1;
        if (defaultSize.y / defaultSize.x > size.y / size.x)
        {
            Debug.Log("横長");
            scale = defaultSize.y / size.y;
        }
        else
        {
            Debug.Log("縦長");
            scale = defaultSize.x / size.x;
        }
        transform.localScale = Vector3.one * scale;
    }
}
