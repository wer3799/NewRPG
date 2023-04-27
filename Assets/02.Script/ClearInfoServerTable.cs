using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using LitJson;
using System;
using System.Text;
using UniRx;
using CodeStage.AntiCheat.ObscuredTypes;

public class ClearInfoServerTable
{
    public static string Indate;
    public const string tableName = "ClearInfo";

    private ReactiveDictionary<string, ReactiveProperty<double>> tableDatas = new ReactiveDictionary<string, ReactiveProperty<double>>();

    public ReactiveDictionary<string, ReactiveProperty<double>> TableDatas => tableDatas;

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

                var table = TableManager.Instance.mainContents.dataArray;

                for (int i = 0; i < table.Length; i++)
                {
                    tableDatas.Add(table[i].Contentsname, new ReactiveProperty<double>(0d));
                    defultValues.Add(table[i].Contentsname, 0d);
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

                var table = TableManager.Instance.mainContents.dataArray;

                for (int i = 0; i < table.Length; i++)
                {
                    if (data.Keys.Contains(table[i].Contentsname))
                    {
                        //값로드
                        var value = double.Parse(data[table[i].Contentsname][ServerData.format_Number].ToString());

                        defultValues.Add(table[i].Contentsname, value);
                        tableDatas.Add(table[i].Contentsname, new ReactiveProperty<double>(value));
                    }
                    else
                    {
                        tableDatas.Add(table[i].Contentsname, new ReactiveProperty<double>(0d));
                        defultValues.Add(table[i].Contentsname, 0d);
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

    public void UpData(string key, double value)
    {
        tableDatas[key].Value = value;


        Param defultValues = new Param();

        defultValues.Add(key, tableDatas[key].Value);

        SendQueue.Enqueue(Backend.GameData.UpdateV2, tableName, Indate, Backend.UserInDate, defultValues, bro =>
        {
            if (bro.IsSuccess() == false)
            {
                ServerData.ShowCommonErrorPopup(bro, () => { UpData(key, value); });
                return;
            }
        });
    }
}