using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using BackEnd.Game.GameInfo;
using LitJson;
using System;
using UniRx;

public class GoldAbilServerTable 
{
    public static string Indate;
    public const string tableName = "GoldStatusTable";

    private ReactiveDictionary<string, ReactiveProperty<int>> tableDatas = new ReactiveDictionary<string, ReactiveProperty<int>>();
    public ReactiveDictionary<string, ReactiveProperty<int>> TableDatas => tableDatas;

    public ReactiveProperty<int> GetTableData(string key)
    {
        return tableDatas[key];
    }

    public int GetCurrentLevel(string key)
    {
        return tableDatas[key].Value;
    }

    public void Initialize()
    {
        tableDatas.Clear();

        SendQueue.Enqueue(Backend.GameData.GetMyData, tableName, new Where(), callback =>
        {
            // 이후 처리
            if (callback.IsSuccess() == false)
            {
                Debug.LogError("LoadStatusFailed");
                PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, CommonString.DataLoadFailedRetry, Initialize);
                return;
            }

            var rows = callback.Rows();

            //맨처음 초기화
            if (rows.Count <= 0)
            {
                Param defultValues = new Param();

                var table = TableManager.Instance.goldAbilTable.dataArray;

                for (int i = 0; i < table.Length; i++)
                {
                    defultValues.Add(table[i].Stringid, 0d);
                    tableDatas.Add(table[i].Stringid, new ReactiveProperty<int>(0));
                }

                var bro = Backend.GameData.Insert(tableName, defultValues);

                if (bro.IsSuccess() == false)
                {
                    // 이후 처리
                    ServerData.ShowCommonErrorPopup(bro, Initialize);
                    return;
                }
                else
                {
                    var jsonData = bro.GetReturnValuetoJSON();
                    if (jsonData.Keys.Count > 0)
                    {
                        Indate = jsonData[0].ToString();
                    }
                }

                return;
            }
            //나중에 칼럼 추가됐을때 업데이트
            else
            {
                Param defultValues = new Param();
                int paramCount = 0;

                JsonData data = rows[0];

                if (data.Keys.Contains(ServerData.inDate_str))
                {
                    Indate = data[ServerData.inDate_str][ServerData.format_string].ToString();
                }

                var table = TableManager.Instance.goldAbilTable.dataArray;

                for (int i = 0; i < table.Length; i++)
                {
                    if (data.Keys.Contains(table[i].Stringid))
                    {
                        //값로드
                        var value = data[table[i].Stringid][ServerData.format_Number].ToString();
                        tableDatas.Add(table[i].Stringid, new ReactiveProperty<int>(int.Parse(value)));
                    }
                    else
                    {
                        tableDatas.Add(table[i].Stringid, new ReactiveProperty<int>(0));
                        defultValues.Add(table[i].Stringid, 0);
                        paramCount++;
                    }
                }

                if (paramCount != 0)
                {
                    var bro = Backend.GameData.UpdateV2(tableName, Indate, Backend.UserInDate, defultValues);

                    if (bro.IsSuccess() == false)
                    {
                        ServerData.ShowCommonErrorPopup(bro, Initialize);
                        return;
                    }
                }
            }
        });
    }

    public void AddLocalData(string key, int amount)
    {
        tableDatas[key].Value += amount;
    }

    public void UpData(string key, bool LocalOnly)
    {
        if (tableDatas.ContainsKey(key) == false)
        {
            Debug.Log($"Status {key} is not exist");
            return;
        }

        UpData(key, tableDatas[key].Value, LocalOnly);
    }

    public void UpData(string key, int data, bool LocalOnly)
    {
        if (tableDatas.ContainsKey(key) == false)
        {
            Debug.Log($"Growth {key} is not exist");
            return;
        }

        tableDatas[key].Value = data;

        if (LocalOnly == false)
        {
            SyncToServerEach(key);
        }
    }

    public void SyncToServerEach(string key, Action whenSyncSuccess = null, Action whenRequestComplete = null, Action whenRequestFailed = null)
    {
        Param param = new Param();
        param.Add(key, tableDatas[key].Value);

        SendQueue.Enqueue(Backend.GameData.UpdateV2, tableName, Indate, Backend.UserInDate, param, e =>
        {
            whenRequestComplete?.Invoke();

            if (e.IsSuccess())
            {
                whenSyncSuccess?.Invoke();
            }
            else if (e.IsSuccess() == false)
            {
                if (whenRequestFailed != null)
                {
                    whenRequestFailed.Invoke();
                }

                Debug.Log($"Growth {key} sync failed");
                return;
            }
        });
    }

    public void SyncAllData(List<string> ignoreList = null)
    {
        Param param = new Param();

        var e = tableDatas.GetEnumerator();
        while (e.MoveNext())
        {
            if (ignoreList != null && ignoreList.Contains(e.Current.Key) == true) continue;
            param.Add(e.Current.Key, e.Current.Value.Value);
        }

        SendQueue.Enqueue(Backend.GameData.UpdateV2, tableName, Indate, Backend.UserInDate, param, bro =>
        {
#if UNITY_EDITOR
            if (bro.IsSuccess() == false)
            {
                Debug.Log($"SyncAllData {tableName} up failed");
                return;
            }
            else
            {
                Debug.Log($"SyncAllData {tableName} up Complete");
                return;
            }
#endif
        });
    }

    public void SyncAllDataForce()
    {
    }

    public float GetStatusValue(object key, int currentLevel)
    {
        return currentLevel;
    }

    public float GetStatusUpgradePrice(object key, int currentLevel)
    {
        return 1f;
    }
}