using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class BaseCanvasManager : MonoBehaviour
{
    /// <summary>
    /// OnOpenとOnCloseのイベント発火タイミングを設定する
    /// </summary>
    /// <param name="thisScreen">セットする画面のenumを入れてください</param>
    protected void SetScreenAction(ScreenState thisScreen)
    {
        this.ObserveEveryValueChanged(screenState => Variables.screenState)
            .Where(screenState => screenState == thisScreen)
            .Subscribe(screenState => { OnOpen(); })
            .AddTo(this.gameObject);

        this.ObserveEveryValueChanged(screenState => Variables.screenState)
            .Buffer(2, 1)
            .Where(screenState => screenState[0] == thisScreen)
            .Where(screenState => screenState[1] != thisScreen)
            .Subscribe(screenState => { OnClose(); })
            .AddTo(this.gameObject);
    }

    public virtual void OnStart()
    {
    }

    public virtual void OnInitialize()
    {
    }

    /// <summary>
    /// 画面が開かれる瞬間だけ呼ばれる
    /// </summary>
    protected virtual void OnOpen()
    {
    }

    /// <summary>
    /// 画面が閉じられる瞬間だけ呼ばれる
    /// </summary>
    protected virtual void OnClose()
    {
    }

    /*  コピペ用

        public override void OnStart()
        {
            base.SetScreenAction(thisScreen: ScreenState.RESULT); 
        }

        public override void OnInitialize()
        {
        }

        protected override void OnOpen()
        {
        }

        protected override void OnClose()
        {
        }
        
    */
}
