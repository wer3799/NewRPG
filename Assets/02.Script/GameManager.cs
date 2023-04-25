using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using CodeStage.AntiCheat.ObscuredTypes;
using System;

//

public enum ContentsWhere
{
    None,
    MainContentsMenu,
}

public class GameManager : SingletonMono<GameManager>
{
    private void Start()
    {
        ResourcesContainer.Initialize();
    }
}