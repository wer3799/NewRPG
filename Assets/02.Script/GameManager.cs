using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using CodeStage.AntiCheat.ObscuredTypes;
using System;
//
public class GameManager : SingletonMono<GameManager>
{
    public enum ContentsType
    {
        NormalField
    }

    public static ContentsType contentsType = ContentsType.NormalField;

}
