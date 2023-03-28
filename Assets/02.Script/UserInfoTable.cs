using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using LitJson;
using System;
using UniRx;
using CodeStage.AntiCheat.ObscuredTypes;

public class UserInfoTable
{
    public static string Indate;
    public const string tableName = "UserInfoTable";

    public const string LastLogin = "LastLogin";
    public const string CurrentStage = "CurrentStage";

    public double currentServerDate;
    public DateTime currentServerTime { get; private set; }
    public ReactiveCommand whenServerTimeUpdated = new ReactiveCommand();
    public const string sleepRewardSavedTime = "sleepRewardSavedTime";

    private Dictionary<string, double> tableSchema = new Dictionary<string, double>()
    {
        { LastLogin, 0f },
        { CurrentStage, 0f },
        { sleepRewardSavedTime, 0f },
    };

    private Dictionary<string, ReactiveProperty<double>>
        tableDatas = new Dictionary<string, ReactiveProperty<double>>();

    public Dictionary<string, ReactiveProperty<double>> TableDatas => tableDatas;

    public ReactiveProperty<double> GetTableData(string key)
    {
        return tableDatas[key];
    }

    public ReactiveCommand WhenDateChanged = new ReactiveCommand();

    public void Initialize()
    {
        tableDatas.Clear();

        SendQueue.Enqueue(Backend.GameData.GetMyData, tableName, new Where(), callback =>
        {
            // 이후 처리
            if (callback.IsSuccess() == false)
            {
                PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, CommonString.DataLoadFailedRetry,
                    Initialize);
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
                    if (e.Current.Key != LastLogin)
                    {
                        defultValues.Add(e.Current.Key, e.Current.Value);
                        tableDatas.Add(e.Current.Key, new ReactiveProperty<double>(e.Current.Value));
                    }
                    else
                    {
                        BackendReturnObject servertime = Backend.Utils.GetServerTime();

                        string time = servertime.GetReturnValuetoJSON()["utcTime"].ToString();
                        DateTime currentServerTime = DateTime.Parse(time).ToUniversalTime().AddHours(9);

                        currentServerDate = (double)Utils.ConvertToUnixTimestamp(currentServerTime);

                        defultValues.Add(e.Current.Key, (double)currentServerDate);
                        tableDatas.Add(e.Current.Key, new ReactiveProperty<double>((double)currentServerDate));
                    }
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

                    // data.
                    // statusIndate = data[DatabaseManager.inDate_str][DatabaseManager.format_string].ToString();
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

                var e = tableSchema.GetEnumerator();

                for (int i = 0; i < data.Keys.Count; i++)
                {
                    while (e.MoveNext())
                    {
                        if (data.Keys.Contains(e.Current.Key))
                        {
                            //값로드
                            var value = data[e.Current.Key][ServerData.format_Number].ToString();
                            tableDatas.Add(e.Current.Key, new ReactiveProperty<double>(double.Parse(value)));
                        }
                        else
                        {
                            defultValues.Add(e.Current.Key, e.Current.Value);
                            tableDatas.Add(e.Current.Key, new ReactiveProperty<double>(e.Current.Value));

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
                        return; //
                    }
                }
            }
        });
    }

    public void UpData(string key, bool LocalOnly)
    {
        if (tableDatas.ContainsKey(key) == false)
        {
            Debug.Log($"UserInfoTable {key} is not exist");
            return;
        }

        UpData(key, tableDatas[key].Value, LocalOnly);
    }

    public void UpData(string key, double data, bool LocalOnly, Action failCallBack = null)
    {
        if (tableDatas.ContainsKey(key) == false)
        {
            Debug.Log($"UserInfoTable {key} is not exist");
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
                    failCallBack?.Invoke();
                    Debug.LogError($"UserInfoTable {key} up failed");
                    return;
                }
            });
        }
    }

    public void AutoUpdateRoutine()
    {
        UpdateLastLoginTime();
    }

    private bool isFirstInit = true;

    public void UpdateLastLoginTime()
    {
        SendQueue.Enqueue(Backend.Utils.GetServerTime, (bro) =>
        {
            var isSuccess = bro.IsSuccess();
            var statusCode = bro.GetStatusCode();
            var returnValue = bro.GetReturnValue();

            if (isSuccess && statusCode.Equals("200") && returnValue != null)
            {
                string time = bro.GetReturnValuetoJSON()["utcTime"].ToString();

                currentServerTime = DateTime.Parse(time).ToUniversalTime().AddHours(9);

                whenServerTimeUpdated.Execute();

                currentServerDate = (double)Utils.ConvertToUnixTimestamp(currentServerTime);

                //day check
                DateTime savedDate = Utils.ConvertFromUnixTimestamp(tableDatas[LastLogin].Value - 2f);

                if (isFirstInit)
                {
                    isFirstInit = false;
                    int elapsedTime = (int)(currentServerTime - savedDate).TotalSeconds;

                    //최소조건 안됨 (시간,첫 접속)
                    if (elapsedTime < GameBalance.sleepRewardMinValue ||
                        ServerData.userInfoTable.GetTableData(UserInfoTable.CurrentStage).Value == -1)
                    {
                        return;
                    }
                    else
                    {
                        //서버에 저장시켜봄
                        Param userInfoParam = new Param();

                        ServerData.userInfoTable.tableDatas[UserInfoTable.sleepRewardSavedTime].Value += elapsedTime;

                        userInfoParam.Add(sleepRewardSavedTime,
                            ServerData.userInfoTable.tableDatas[UserInfoTable.sleepRewardSavedTime].Value);

                        var returnBro = Backend.GameData.UpdateV2(tableName, Indate, Backend.UserInDate, userInfoParam);

                        if (returnBro.IsSuccess() == false)
                        {
                            PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, "네트워크가 불안정 합니다.\n앱을 재실행 합니다.",
                                () => { Utils.RestartApplication(); });

                            return;
                        }
                    }
                }

                //week check
                int currentWeek = Utils.GetWeekNumber(currentServerTime);

                int savedWeek = Utils.GetWeekNumber(savedDate);

                if (savedDate.Day != currentServerTime.Day)
                {
                    Debug.LogError("@@@Day Changed!");
                    if (savedDate.Month != currentServerTime.Month)
                    {
                        Debug.LogError("@@@Month Changed!");
                    }

                    //날짜 바뀜
                    DateChanged(currentServerTime.Day, savedWeek != currentWeek,
                        savedDate.Month != currentServerTime.Month);
                }
                else
                {
                    UpdateLastLoginOnly();
                }
            }
        });
    }

    private void UpdateLastLoginOnly()
    {
        List<TransactionValue> transactionList = new List<TransactionValue>();

        ServerData.userInfoTable.GetTableData(UserInfoTable.LastLogin).Value = (double)currentServerDate;

        Param userInfoParam = new Param();
        userInfoParam.Add(UserInfoTable.LastLogin,
            Math.Truncate(ServerData.userInfoTable.GetTableData(UserInfoTable.LastLogin).Value));
        transactionList.Add(TransactionValue.SetInsert(UserInfoTable.tableName, userInfoParam));

        ServerData.SendTransaction(transactionList, true);
    }


    private void DateChanged(int day, bool weekChanged, bool monthChanged)
    {
        WhenDateChanged.Execute();

        List<TransactionValue> transactionList = new List<TransactionValue>();

        //일일초기화
        Param userInfoParam = new Param();
        
        ServerData.userInfoTable.GetTableData(UserInfoTable.LastLogin).Value = (double)currentServerDate;
        
        userInfoParam.Add(UserInfoTable.LastLogin, Math.Truncate(ServerData.userInfoTable.GetTableData(UserInfoTable.LastLogin).Value));

        if (weekChanged)
        {
        }

        if (monthChanged)
        {
        }

        transactionList.Add(TransactionValue.SetUpdateV2(UserInfoTable.tableName,UserInfoTable.Indate,Backend.UserInDate,userInfoParam));
        
        ServerData.SendTransaction(transactionList, false);
    }

    private void WeekChanged()
    {
    }

    private void MonthChanged()
    {
    }

    public bool IsHotTime()
    {
        // if (currentServerTime.DayOfWeek != DayOfWeek.Sunday && currentServerTime.DayOfWeek != DayOfWeek.Saturday)
        // {
        //     int currentHour = currentServerTime.Hour;
        //     return currenxtHour >= GameBalance.HotTime_Start && currentHour < GameBalance.HotTime_End;
        // }
        // else
        // {
        //     int currentHour = currentServerTime.Hour;
        //     return currentHour >= GameBalance.HotTime_Start_Weekend && currentHour < GameBalance.HotTime_End;
        // }
        return false;
    }

    public bool IsWeekend()
    {
        return currentServerTime.DayOfWeek == DayOfWeek.Sunday || currentServerTime.DayOfWeek == DayOfWeek.Saturday;
    }
}
//