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

}