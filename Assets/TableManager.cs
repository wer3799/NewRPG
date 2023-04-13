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
    
    public int GetLastStageIdx()
    {
        return stageMap.dataArray[stageMap.dataArray.Length - 1].Id;
    }
    
    public EnemyTable enemyTable;
    private Dictionary<string, EnemyTableData> enemyData = null;

    public Dictionary<string, EnemyTableData> EnemyData
    {
        get
        {
            LoadEnemyData();
            return enemyData;
        }
    }

    private void LoadEnemyData()
    {
        if (enemyData != null) return;

        enemyData = new Dictionary<string, EnemyTableData>();

        for (int i = 0; i < enemyTable.dataArray.Length; i++)
        {
            enemyData.Add(enemyTable.dataArray[i].Name, enemyTable.dataArray[i]);
        }
    }
    
    
    public SkillTable skillTable;
    
    private Dictionary<int, SkillTableData> skillData = null;

    public Dictionary<int, SkillTableData> SkillData
    {
        get
        {
            LoadSkillData();
            return skillData;
        }
    }

    private void LoadSkillData()
    {
        if (skillData != null) return;

        skillData = new Dictionary<int, SkillTableData>();

        for (int i = 0; i < skillTable.dataArray.Length; i++)
        {
            skillData.Add(skillTable.dataArray[i].Id, skillTable.dataArray[i]);
        }
    }


    public GoldAbilTable goldAbilTable;

    public Goods goodsTable;

}