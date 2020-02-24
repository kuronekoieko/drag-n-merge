using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class BackgroundToggleController : MonoBehaviour
{
    /*
    【uGUI】ToggleのON/OFF時にToggleの色を変える
    https://qiita.com/daria_sieben/items/920c42c9046678baa971
    */
    [SerializeField] Image bGImage;
    [SerializeField] Text coinCountText;
    [SerializeField] RectTransform coinCountRT;
    [SerializeField] RectTransform toggleRT;
    [SerializeField] Button getButton;

    Toggle toggle;
    int index;
    BackgroundData backgroundData;

    public void OnStart(ToggleGroup toggleGroup, int index)
    {
        this.index = index;
        backgroundData = BackgroundDataSO.i.backgroundDatas[index];
        bGImage.sprite = backgroundData.sprite;
        coinCountText.text = backgroundData.coinCount.ToString();
        toggle = GetComponent<Toggle>();
        toggle.group = toggleGroup;
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
        getButton.onClick.AddListener(OnClickGetButton);

        this.ObserveEveryValueChanged(isPossessed => SaveData.i.possessedBackgrounds[index].isPossessed)
            .Subscribe(isPossessed => ShowPossessed(isPossessed))
            .AddTo(this.gameObject);
    }

    void OnToggleValueChanged(bool isOn)
    {
        SaveData.i.possessedBackgrounds[index].isSelected = isOn;
        SaveDataManager.i.Save();
    }

    public void OnOpen()
    {
        toggle.isOn = SaveData.i.possessedBackgrounds[index].isSelected;
    }

    void OnClickGetButton()
    {
        if (SaveData.i.coinCount < backgroundData.coinCount) { return; }
        SaveData.i.possessedBackgrounds[index].isPossessed = true;
        SaveData.i.coinCount -= backgroundData.coinCount;
        SaveDataManager.i.Save();
    }

    void ShowPossessed(bool isPossessed)
    {
        coinCountRT.gameObject.SetActive(!isPossessed);
        getButton.gameObject.SetActive(!isPossessed);
        toggleRT.gameObject.SetActive(isPossessed);
        toggle.interactable = isPossessed;
    }
}
