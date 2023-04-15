using BackEnd;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class GrowthManager : SingletonMono<GrowthManager>
{
    public ReactiveProperty<double> maxExp = new ReactiveProperty<double>();

    private Coroutine levelUpRoutine;

    private void Start()
    {
        PlayerData.Instance.WhenUserDataLoadComplete.AsObservable().Subscribe(e => { Subscribe(); }).AddTo(this);
    }

    private void Subscribe()
    {
        ServerData.growthTable.GetTableData(GrowthTable.Level).AsObservable().Subscribe(e =>
        {
            Debug.LogError("Max expUpdated");
            maxExp.Value = GameDataCalculator.GetMaxExp((int)ServerData.growthTable.GetTableData(GrowthTable.Level).Value);
        }).AddTo(this);
    }

    public void GetExp(float exp)
    {
        ServerData.growthTable.GetTableData(GrowthTable.Exp).Value += exp;

        if (CanLevelUp())
        {
            if (levelUpRoutine != null)
            {
                StopCoroutine(levelUpRoutine);
            }

            StartCoroutine(LevelUpRoutine());
        }

        UiExpGauge.Instance.updateFlag = true;
    }

    private bool CanLevelUp()
    {
        return ServerData.growthTable.GetTableData(GrowthTable.Exp).Value >= maxExp.Value;
    }

    private IEnumerator LevelUpRoutine()
    {
        while (CanLevelUp() == true)
        {
            LevelUpLocal();

            Debug.LogError("LevelUp!");

            yield return null;
        }

        UiExpGauge.Instance.updateFlag = true;
    }

    private void LevelUpLocal()
    {
        ServerData.growthTable.GetTableData(GrowthTable.Exp).Value -= maxExp.Value;

        ServerData.growthTable.GetTableData(GrowthTable.Level).Value += 1;

        ServerData.growthTable.GetTableData(GrowthTable.LevelStatPoint).Value += GameBalance.levelUpStatPoint;
    }

    public void SyncLevelUpDatas()
    {
        List<TransactionValue> transactionList = new List<TransactionValue>();

        Param growthParam = new Param();

        growthParam.Add(GrowthTable.Exp, Math.Truncate(ServerData.growthTable.GetTableData(GrowthTable.Exp).Value));

        growthParam.Add(GrowthTable.Level, ServerData.growthTable.GetTableData(GrowthTable.Level).Value);

        growthParam.Add(GrowthTable.LevelStatPoint, ServerData.growthTable.GetTableData(GrowthTable.LevelStatPoint).Value);

        transactionList.Add(TransactionValue.SetUpdateV2(GrowthTable.tableName, GrowthTable.Indate, Backend.UserInDate, growthParam));

        ServerData.SendTransaction(transactionList, successCallBack: () => { });
    }
}