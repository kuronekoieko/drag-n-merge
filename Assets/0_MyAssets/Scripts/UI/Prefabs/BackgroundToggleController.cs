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
    [SerializeField] Image toggleBGImage;
    [SerializeField] RectTransform coinCountRT;
    [SerializeField] Image lockBGImage;
    [SerializeField] Image hideBGImage;
    [SerializeField] Image lockIconImage;
    [SerializeField] Button getButton;
    Toggle toggle;
    int index;
    BackgroundData backgroundData;
    readonly Vector2 defaultSize = new Vector2(168.6f, 195f);
    BGToggleState bGToggleState;
    SkinCanvasManager skinCanvasManager;
    ToggleGroup toggleGroup;
    public void OnStart(ToggleGroup toggleGroup, int index, SkinCanvasManager skinCanvasManager)
    {
        this.toggleGroup = toggleGroup;
        this.index = index;
        this.skinCanvasManager = skinCanvasManager;
        backgroundData = BackgroundDataSO.i.backgroundDatas[index];
        bGImage.sprite = backgroundData.sprite;
        SetSize();
        coinCountText.text = backgroundData.coinCount.ToString();
        toggle = GetComponent<Toggle>();
        toggle.group = toggleGroup;
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
        getButton.onClick.AddListener(OnClickGetButton);

        this.ObserveEveryValueChanged(bGToggleState => SaveData.i.possessedBackgrounds[index].bGToggleState)
            .Subscribe(bGToggleState => ShowPossessed(bGToggleState))
            .AddTo(this.gameObject);
    }

    void OnToggleValueChanged(bool isOn)
    {
        SaveData.i.possessedBackgrounds[index].isSelected = isOn;
        SaveDataManager.i.Save();

        if (isOn) { AudioManager.i.PlayOneShot(6); }
    }

    public void OnOpen()
    {
        toggle.isOn = SaveData.i.possessedBackgrounds[index].isSelected;
    }

    void OnClickGetButton()
    {
        if (SaveData.i.coinCount < backgroundData.coinCount) { return; }
        SaveData.i.possessedBackgrounds[index].bGToggleState = BGToggleState.Unlocked;
        toggle.group = toggleGroup;
        SaveData.i.coinCount -= backgroundData.coinCount;
        SaveDataManager.i.Save();
        skinCanvasManager.OpenHiddenLock();
        AudioManager.i.PlayOneShot(7);
    }

    void ShowPossessed(BGToggleState bGToggleState)
    {
        toggleBGImage.gameObject.SetActive(true);
        coinCountRT.gameObject.SetActive(true);
        lockBGImage.gameObject.SetActive(true);
        hideBGImage.gameObject.SetActive(true);
        lockIconImage.gameObject.SetActive(true);
        getButton.gameObject.SetActive(true);
        toggle.interactable = true;

        switch (bGToggleState)
        {
            case BGToggleState.Unlocked:
                coinCountRT.gameObject.SetActive(false);
                lockBGImage.gameObject.SetActive(false);
                hideBGImage.gameObject.SetActive(false);
                lockIconImage.gameObject.SetActive(false);
                getButton.gameObject.SetActive(false);
                break;
            case BGToggleState.ShownLock:
                toggleBGImage.gameObject.SetActive(false);
                hideBGImage.gameObject.SetActive(false);
                toggle.interactable = false;
                break;
            case BGToggleState.HiddenLock:
                toggleBGImage.gameObject.SetActive(false);
                lockBGImage.gameObject.SetActive(false);
                getButton.gameObject.SetActive(false);
                toggle.interactable = false;
                break;
            default:
                break;
        }
    }

    void SetSize()
    {

        bGImage.SetNativeSize();
        Vector2 size = bGImage.rectTransform.sizeDelta;
        bGImage.transform.localScale = Vector3.one;
        float scale = 1;
        if (defaultSize.y / defaultSize.x > size.y / size.x)
        {
            //Debug.Log("横長");
            scale = defaultSize.y / size.y;
        }
        else
        {
            //Debug.Log("縦長");
            scale = defaultSize.x / size.x;
        }
        bGImage.transform.localScale = Vector3.one * scale;
    }
}
