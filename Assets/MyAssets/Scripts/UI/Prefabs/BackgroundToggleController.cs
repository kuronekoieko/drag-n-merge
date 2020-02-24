using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class BackgroundToggleController : MonoBehaviour
{
    [SerializeField] Image bGImage;
    [SerializeField] Text coinCountText;
    Toggle toggle;
    int index;

    public bool isOn
    {
        set { toggle.isOn = value; }
        get { return toggle.isOn; }
    }

    public BackgroundData backgroundData { get; private set; }

    public void OnStart(BackgroundData backgroundData, ToggleGroup toggleGroup, int index)
    {
        this.index = index;
        this.backgroundData = backgroundData;
        bGImage.sprite = backgroundData.sprite;
        coinCountText.text = backgroundData.coinCount.ToString();
        toggle = GetComponent<Toggle>();
        toggle.group = toggleGroup;
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    void OnToggleValueChanged(bool isOn)
    {
        SaveData.i.possessedBackgrounds[index].isSelected = isOn;
        SaveDataManager.i.Save();
    }

}
