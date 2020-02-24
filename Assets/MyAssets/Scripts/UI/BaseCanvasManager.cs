using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class BaseCanvasManager : MonoBehaviour
{
    public ScreenState screenState { get; set; }

    public virtual void Open()
    {
    }

    public virtual void Close()
    {

    }

    public virtual void OnStart()
    {
    }

    public virtual void OnInitialize()
    {
    }

    /*  コピペ用

        public override void OnStart()
        {
            base.SetScreenAction(thisScreen: ScreenState.); 
        }

        public override void OnInitialize()
        {
        }

        public override void Open()
        {
        }

        public override void Close()
        {
        }
        
    */
}
