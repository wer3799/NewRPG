using System;
using BackEnd;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class SaveManager : SingletonMono<SaveManager>
{
    private WaitForSeconds updateDelay = new WaitForSeconds(60.0f);

    private WaitForSeconds versionCheckDelay = new WaitForSeconds(3600.0f);

    private WaitForSeconds tockenRefreshDelay = new WaitForSeconds(43200f);

    private void Start()
    {
        Subscribe();
    }

    private void Subscribe()
    {
        PlayerData.Instance.WhenUserDataLoadComplete.AsObservable().Subscribe(e => { StartAutoSave(); }).AddTo(this);
    }

    public void StartAutoSave()
    {
        StartCoroutine(AutoSaveRoutine());
        StartCoroutine(TockenRefreshRoutine());
        StartCoroutine(VersionCheckRoutine());
    }

    private IEnumerator VersionCheckRoutine()
    {
        while (true)
        {
            yield return versionCheckDelay;

            CheckClientVersion();
        }
    }

    private void CheckClientVersion()
    {
        SendQueue.Enqueue(Backend.Utils.GetLatestVersion, bro =>
        {
            if (bro.IsSuccess())
            {
                int clientVersion = int.Parse(Application.version);

                var jsonData = bro.GetReturnValuetoJSON();
                string serverVersion = jsonData["version"].ToString();

                //버전이 높거나 같음
                if (clientVersion >= int.Parse(serverVersion))
                {
                }
                else
                {
                    PopupManager.Instance.ShowVersionUpPopup(CommonString.Notice, "업데이트 버전이 있습니다. 스토어로 이동합니다.\n업데이트 버튼이 활성화 되지 않은 경우\n구글 플레이 스토어를 닫았다가 다시 열어 보세요!", () =>
                    {
                        SyncAutoSaveData();
#if UNITY_ANDROID
                        Application.OpenURL("https://play.google.com/store/apps/details?id=com.DragonGames.yoyo&hl=ko");
#endif

#if UNITY_IOS
                    Application.OpenURL("itms-apps://itunes.apple.com/app/id1587651736");
#endif
                    }, false);
                }
            }
        });
    }

    private IEnumerator TockenRefreshRoutine()
    {
        while (true)
        {
            yield return tockenRefreshDelay;
            BackEnd.Backend.BMember.LoginWithTheBackendToken(e =>
            {
                if (e.IsSuccess())
                {
                    Debug.Log("토큰 갱신 성공");
                }
                else
                {
                    Debug.Log("토큰 갱신 실패");
                }
            });
        }
    }

    private IEnumerator AutoSaveRoutine()
    {
        while (true)
        {
            yield return updateDelay;
            SyncAutoSaveData();
        }
    }


    //SendQueue에서 저장
    public void SyncAutoSaveData()
    {
        GrowthManager.Instance.SyncLevelUpDatas();

        ServerData.goodsTable.SyncExceptIgnoreList();

        ServerData.userInfoTable.UpdateLastLoginTime();
    }

    private void OnApplicationQuit()
    {
        SyncAutoSaveData();

        SetOfflineRewardAlarm();
    }

    public void SetOfflineRewardAlarm()
    {
        if (SettingData.ShowSleepPush.Value == 1)
        {
            GleyNotifications.SendNotification("휴식보상", "휴식 보상이 가득 찼어요!(24시간)", new System.TimeSpan(24, 0, 0));
        }
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SyncAutoSaveData();
        }
    }
#endif
}