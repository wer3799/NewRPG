using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using LitJson;
using System;
using UniRx;


public class NorigaeServerTable : MonoBehaviour
{
    public static string Indate;
    public static string tableName = "Norigae";


    private ReactiveDictionary<string, EquipmentServerData> tableDatas = new ReactiveDictionary<string, EquipmentServerData>();

    public ReactiveDictionary<string, EquipmentServerData> TableDatas => tableDatas;

    public float GetWeaponEffectValue(string id, float baseValue, float addValue, int level = -1)
    {
        if (level == -1)
        {
            return baseValue + addValue * tableDatas[id].level.Value;
        }
        else
        {
            return baseValue + addValue * level;
        }
    }

    public EquipmentServerData GetWeaponData(string idx)
    {
        if (tableDatas.TryGetValue(idx, out var data))
        {
            return data;
        }
        else
        {
            return null;
        }
    }

    public int GetCount(string idx)
    {
        return tableDatas[idx].amount.Value;
    }

    public float GetWeaponLevelUpPrice(string idx)
    {
        int level = tableDatas[idx].level.Value;
        int id = tableDatas[idx].idx;
        level += 1;

        return Mathf.Pow(level, 3.35f + (float)id * 0.015f);

        // if (id < 20)
        // {
        // }
        // //요물은 업글이 좀 비싸
        // else if (id == 20)
        // {
        //     return Mathf.Pow(level, 3.45f + (float)id * 0.015f);
        // }
        // //야차
        // else if (id == 21)
        // {
        //     return Mathf.Pow(level, 3.65f + (float)id * 0.015f);
        // }
        // //필멸
        // else
        // {
        //     return Mathf.Pow(level, 3.80f + (float)id * 0.015f);
        // }
    }

    public void Initialize()
    {
        tableDatas.Clear();

        SendQueue.Enqueue(Backend.GameData.GetMyData, tableName, new Where(), callback =>
        {
            // 이후 처리
            if (callback.IsSuccess() == false)
            {
                Debug.LogError("LoadWeaponFail");
                PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, CommonString.DataLoadFailedRetry, Initialize);
                return;
            }

            var rows = callback.Rows();

            //맨처음 초기화
            if (rows.Count <= 0)
            {
                Param defultValues = new Param();

                var table = TableManager.Instance.norigae.dataArray;

                for (int i = 0; i < table.Length; i++)
                {
                    var equipData = new EquipmentServerData();
                    equipData.idx = table[i].Id;
                    equipData.hasItem = new ReactiveProperty<int>(0);
                    equipData.level = new ReactiveProperty<int>(0);
                    equipData.amount = new ReactiveProperty<int>(0);
                    equipData.getReward0 = new ReactiveProperty<int>(0);
                    equipData.getReward1 = new ReactiveProperty<int>(0);

                    tableDatas.Add(table[i].Stringid, equipData);
                    defultValues.Add(table[i].Stringid, equipData.ConvertToString());
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

                var table = TableManager.Instance.norigae.dataArray;

                for (int i = 0; i < table.Length; i++)
                {
                    if (data.Keys.Contains(table[i].Stringid))
                    {
                        //값로드
                        var value = data[table[i].Stringid][ServerData.format_string].ToString();

                        var equipData = new EquipmentServerData();

                        var splitData = value.Split(',');

                        equipData.idx = int.Parse(splitData[0]);
                        equipData.hasItem = new ReactiveProperty<int>(int.Parse(splitData[1]));
                        equipData.level = new ReactiveProperty<int>(int.Parse(splitData[2]));
                        equipData.amount = new ReactiveProperty<int>(int.Parse(splitData[3]));
                        equipData.getReward0 = new ReactiveProperty<int>(0);
                        equipData.getReward1 = new ReactiveProperty<int>(0);

                        defultValues.Add(table[i].Stringid, equipData.ConvertToString());
                        tableDatas.Add(table[i].Stringid, equipData);
                    }
                    else
                    {
                        var equipData = new EquipmentServerData();
                        equipData.idx = table[i].Id;
                        equipData.hasItem = new ReactiveProperty<int>(0);
                        equipData.level = new ReactiveProperty<int>(0);
                        equipData.amount = new ReactiveProperty<int>(0);
                        equipData.getReward0 = new ReactiveProperty<int>(0);
                        equipData.getReward1 = new ReactiveProperty<int>(0);

                        tableDatas.Add(table[i].Stringid, equipData);
                        defultValues.Add(table[i].Stringid, equipData.ConvertToString());
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

    public void UpData(NorigaeData equipData, int addNum)
    {
        if (tableDatas[equipData.Stringid].hasItem.Value == 0)
        {
            tableDatas[equipData.Stringid].hasItem.Value = 1;
        }

        tableDatas[equipData.Stringid].amount.Value += addNum;
    }

    public void SyncToServerEach(string key)
    {
        Param defultValues = new Param();

        defultValues.Add(key, tableDatas[key].ConvertToString());

        SendQueue.Enqueue(Backend.GameData.UpdateV2, tableName, Indate, Backend.UserInDate, defultValues, bro =>
        {
            if (bro.IsSuccess() == false)
            {
                ServerData.ShowCommonErrorPopup(bro, () => { SyncToServerEach(key); });
                return;
            }
        });
    }

    public void SyncToServerAll(List<int> updateList = null)
    {
        Param defultValues = new Param();

        var table = TableManager.Instance.norigae.dataArray;

        for (int i = 0; i < table.Length; i++)
        {
            if (updateList != null && updateList.Contains(table[i].Id) == false) continue;

            string key = table[i].Stringid;

            defultValues.Add(key, tableDatas[key].ConvertToString());
        }

        SendQueue.Enqueue(Backend.GameData.UpdateV2, tableName, Indate, Backend.UserInDate, defultValues, bro =>
        {
            if (bro.IsSuccess() == false)
            {
                ServerData.ShowCommonErrorPopup(bro, () => { SyncToServerAll(updateList); });
                return;
            }
        });
    }
}