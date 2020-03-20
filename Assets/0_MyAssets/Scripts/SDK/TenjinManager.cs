﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TenjinManager : MonoBehaviour
{
    string API_KEY = "EDZBXKASDVB6NXDGYBPZUSZYNX1HJKUW";
    public void OnStart()
    {
#if !UNITY_EDITOR
        BaseTenjin instance = Tenjin.getInstance(API_KEY);
        instance.Connect();
#endif
    }

    void OnApplicationPause(bool pauseStatus)
    {
#if !UNITY_EDITOR
        if (pauseStatus)
        {
            //do nothing
        }
        else
        {
            BaseTenjin instance = Tenjin.getInstance(API_KEY);
            instance.Connect();
        }
#endif
    }
}