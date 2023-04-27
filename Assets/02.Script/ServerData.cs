using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using System;

public static class ServerData
{
    public static UserInfoTable userInfoTable { get; private set; } = new UserInfoTable();
    public static GrowthTable growthTable { get; private set; } = new GrowthTable();
    public static GoodsTable goodsTable { get; private set; } = new GoodsTable();
    public static SkillServerTable skillServerTable { get; private set; } = new SkillServerTable();

    public static GoldAbilServerTable goldAbilServerTable { get; private set; } = new GoldAbilServerTable();

    public static MainWeaponServerTable mainWeaponServerTable { get; private set; } = new MainWeaponServerTable();
    public static SubWeaponServerTable subWeaponServerTable { get; private set; } = new SubWeaponServerTable();
    public static CharmServerTable charmServerTable { get; private set; } = new CharmServerTable();
    public static NorigaeServerTable norigaeServerTable { get; private set; } = new NorigaeServerTable();
    public static PresetServerTable presetServerTable { get; private set; } = new PresetServerTable();
    public static ClearInfoServerTable clearInfoServerTable { get; private set; } = new ClearInfoServerTable();

    #region string

    public static string inDate_str = "inDate";
    public static string format_string = "S";
    public static string format_Number = "N";
    public static string format_bool = "BOOL";
    public static string format_dic = "M";
    public static string format_list = "L";

    //  BOOL bool boolean 형태의 데이터가 이에 해당됩니다.
    //  N   numbers int, float, double 등 모든 숫자형 데이터는 이에 해당됩니다.
    //  S   string  string 형태의 데이터가 이에 해당됩니다.
    //  L list    list 형태의 데이터가 이에 해당됩니다.
    //  M map map, dictionary 형태의 데이터가 이에 해당됩니다.
    //  NULL    null	값이 존재하지 않는 경우 이에 해당됩니다.

    #endregion

    public static void LoadTables()
    {
        userInfoTable.Initialize();
        goodsTable.Initialize();
        skillServerTable.Initialize();
        goldAbilServerTable.Initialize();
        growthTable.Initialize();
        mainWeaponServerTable.Initialize();
        subWeaponServerTable.Initialize();
        charmServerTable.Initialize();
        norigaeServerTable.Initialize();
        presetServerTable.Initialize();
        clearInfoServerTable.Initialize();
    }

    public static void ShowCommonErrorPopup(BackendReturnObject bro, Action retryCallBack)
    {
        PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, $"{CommonString.DataLoadFailedRetry}\n{bro.GetStatusCode()}", retryCallBack);
    }

    public static void SendTransaction(List<TransactionValue> transactionList, bool retry = true, Action completeCallBack = null, Action successCallBack = null)
    {
        SendQueue.Enqueue(Backend.GameData.TransactionWriteV2, transactionList, (bro) =>
        {
            if (bro.IsSuccess())
            {
                successCallBack?.Invoke();
            }
            else
            {
                Debug.LogError($"SendTransaction error!!! {bro.GetMessage()}");

                if (retry)
                {
                    CoroutineExecuter.Instance.StartCoroutine(TransactionRetryRoutine(transactionList));
                }
                else
                {
                    PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, "네트워크가 불안정 합니다.\n앱을 재실행 합니다.", () => { Utils.RestartApplication(); });
                }
            }

            completeCallBack?.Invoke();
        });
    }

    private static WaitForSeconds retryWs = new WaitForSeconds(3.0f);

    private static IEnumerator TransactionRetryRoutine(List<TransactionValue> transactionList)
    {
        yield return retryWs;
        SendTransaction(transactionList, retry: false);
    }
}