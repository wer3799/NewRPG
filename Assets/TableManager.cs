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

    public EnemyTableData GetTableDataByLevel(int level)
    {
        EnemyTableData ret = null;

        var tableData = enemyTable.dataArray;

        for (int i = 0; i < tableData.Length; i++)
        {
            if (level >= tableData[i].Minlevel && level <= tableData[i].Maxlevel)
            {
                ret = tableData[i];
                break;
            }
        }

        if (ret == null)
        {
#if UNITY_EDITOR
            Debug.LogError("@@@@Enemy data load failed");
#endif
        }

        return ret;
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

    public MainWeapon mainWeapon;

    public SubWeapon subWeapon;

    public Charm charm;

    public Norigae norigae;
}