using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using LitJson;
using System;
using UniRx;
using System.Linq;
using CodeStage.AntiCheat.ObscuredTypes;

public class PresetServerTable
{
    public static string Indate;
    public const string tableName = "Preset";

    public const string skill0 = "skill0";
    public const string skill1 = "skill1";
    public const string skill2 = "skill2";


    private Dictionary<string, string> tableSchema = new Dictionary<string, string>()
    {
        { skill0, "0,-1,-1,-1,-1" },
        { skill1, "-1,-1,-1,-1,-1" },
        { skill2, "-1,-1,-1,-1,-1" },
    };

    private Dictionary<string, ReactiveProperty<string>> tableDatas = new Dictionary<string, ReactiveProperty<string>>();

    private Dictionary<string, ReactiveProperty<string>> TableDatas => tableDatas;

    private Dictionary<int, List<int>> skillPresets = new Dictionary<int, List<int>>();

    public void UpdateSkillPresets()
    {
        skillPresets.Clear();

        var preset0 = tableDatas[skill0].Value.Split(',').Select(e => int.Parse(e)).ToList();
        skillPresets.Add(0, preset0);

        var preset1 = tableDatas[skill1].Value.Split(',').Select(e => int.Parse(e)).ToList();
        skillPresets.Add(1, preset1);

        var preset2 = tableDatas[skill2].Value.Split(',').Select(e => int.Parse(e)).ToList();
        skillPresets.Add(2, preset2);
    }

    public List<int> GetSkillPreset(int idx)
    {
        return skillPresets[idx];
    }

    public ReactiveProperty<string> GetTableData(string key)
    {
        return tableDatas[key];
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

                var e = tableSchema.GetEnumerator();

                while (e.MoveNext())
                {
                    defultValues.Add(e.Current.Key, e.Current.Value);
                    tableDatas.Add(e.Current.Key, new ReactiveProperty<string>(e.Current.Value));
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
                
                UpdateSkillPresets();
                
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

                var e = tableSchema.GetEnumerator();

                for (int i = 0; i < data.Keys.Count; i++)
                {
                    while (e.MoveNext())
                    {
                        if (data.Keys.Contains(e.Current.Key))
                        {
                            //값로드
                            var value = data[e.Current.Key][ServerData.format_string].ToString();
                            tableDatas.Add(e.Current.Key, new ReactiveProperty<string>(value));
                        }
                        else
                        {
                            defultValues.Add(e.Current.Key, e.Current.Value);
                            tableDatas.Add(e.Current.Key, new ReactiveProperty<string>(e.Current.Value));
                            paramCount++;
                        }
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
            
            UpdateSkillPresets();
        });
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

    public void UpData(string key, string data, bool LocalOnly)
    {
        if (tableDatas.ContainsKey(key) == false)
        {
            Debug.Log($"Growth {key} is not exist");
            return;
        }

        tableDatas[key].Value = data;

        if (LocalOnly == false)
        {
            Param param = new Param();
            param.Add(key, tableDatas[key].Value);

            SendQueue.Enqueue(Backend.GameData.UpdateV2, tableName, Indate, Backend.UserInDate, param, e =>
            {
                if (e.IsSuccess() == false)
                {
                    Debug.LogError($"Growth {key} up failed");
                    return;
                }
            });
        }
    }
}