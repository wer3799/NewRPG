using CodeStage.AntiCheat.ObscuredTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using UnityEngine.Serialization;

public class TableManager : SingletonMono<TableManager>
{
    public StageMap stageMap;
    public EnemyTable enemyTable;
}