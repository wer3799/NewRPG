using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    void Start()
    {
        Initialize();
    }
    private void InitFrameRate()
    {
        Application.targetFrameRate = 60;
    }

    private void Initialize()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        
        InitFrameRate(); 
        
        SettingData.InitFirst();
    }
}
