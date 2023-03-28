using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using BackEnd.Game.GameInfo;
using LitJson;
using System;
using UniRx;



public class GoodsTable
{
    public static string Indate;
    public const string tableName = "GoodsTable";
    public static string Gold = "Gold";
    public static string Diamond = "Diamond"; //옥
   
    public List<string> ignoreSyncGoodsList = new List<string>()
    {
      
    };


    private Dictionary<string, float> tableSchema = new Dictionary<string, float>()
    {
        {Gold,GameBalance.StartingMoney},
        {Diamond,0f},
      
    };

    private ReactiveDictionary<string, ReactiveProperty<float>> tableDatas = new ReactiveDictionary<string, ReactiveProperty<float>>();
    public ReactiveDictionary<string, ReactiveProperty<float>> TableDatas => tableDatas;

    public ReactiveProperty<float> GetTableData(string key)
    {
        return tableDatas[key];
    }

    public float GetCurrentGoods(string key)
    {
        return tableDatas[key].Value;
    }

    public void GetGold(float amount)
    {
        tableDatas[Gold].Value += amount;
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
                      tableDatas.Add(e.Current.Key, new ReactiveProperty<float>(e.Current.Value));
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
                              tableDatas.Add(e.Current.Key, new ReactiveProperty<float>(float.Parse(value)));
                          }
                          else
                          {
                              defultValues.Add(e.Current.Key, e.Current.Value);
                              tableDatas.Add(e.Current.Key, new ReactiveProperty<float>(e.Current.Value));
                              paramCount++;
                          }
                      }
                  }

                  if (paramCount != 0)
                  {
                      var bro = Backend.GameData.UpdateV2(tableName, Indate,Backend.UserInDate, defultValues);

                      if (bro.IsSuccess() == false)
                      {
                          ServerData.ShowCommonErrorPopup(bro, Initialize);
                          return;
                      }
                  }

              }
          });
    }

    public void AddLocalData(string key, float amount)
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

    public void UpData(string key, float data, bool LocalOnly)
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

        SendQueue.Enqueue(Backend.GameData.UpdateV2, tableName, Indate, Backend.UserInDate,param, e =>
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

        SendQueue.Enqueue(Backend.GameData.UpdateV2, tableName, Indate, Backend.UserInDate,param, bro =>
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
   
   
}
