using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using CodeStage.AntiCheat.ObscuredTypes;
using System;
//
public class GameManager : SingletonMono<GameManager>
{
    public enum InitPlayerPortalPosit
    {
        Left, Right
    }
    public enum ContentsType
    {
        NormalField,
        FireFly,
    }
    public bool IsNormalField => contentsType == ContentsType.NormalField;

    public StageMapData CurrentStageData { get; private set; }

    private ReactiveProperty<int> currentMapIdx = new ReactiveProperty<int>();

    public static ContentsType contentsType { get; private set; }

    public ReactiveCommand whenSceneChanged = new ReactiveCommand();

    public ObscuredInt bossId { get; private set; }
    public ObscuredInt currentTowerId { get; private set; }

    private bool firstInit = true;
   
    public void SetBossId(int bossId)
    {
        this.bossId = bossId;

        RandomizeKey();
    }

    private new void Awake()
    {
        base.Awake();
        SettingData.InitFirst();
    }

    private void RandomizeKey()
    {
        this.bossId.RandomizeCryptoKey();
    }

    public void Initialize()
    {
        Subscribe();
        InitGame();
    }

    private void InitGame()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = 60;
    }

    private void SetFrameRate(int option)
    {
        Application.targetFrameRate = 30 + 15 * option;
#if UNITY_EDITOR
        Debug.LogError($"Frame changed {Application.targetFrameRate}");
#endif
    }

    private void Subscribe()
    {
        AutoManager.Instance.Subscribe();

        currentMapIdx.AsObservable().Subscribe(e =>
        {
            if (!firstInit)
            {
                ServerData.userInfoTable.UpData(UserInfoTable.LastMap, e, true);
            }
            else
            {
                ServerData.userInfoTable.GetTableData(UserInfoTable.LastMap).Value = ServerData.userInfoTable.GetTableData(UserInfoTable.topClearStageId).Value + 1;
                firstInit = false;
            }
        }).AddTo(this);

        SettingData.FrameRateOption.AsObservable().Subscribe(SetFrameRate).AddTo(this);
    }

    private void ClearStage()
    {
        int lastIdx = (int)ServerData.userInfoTable.GetTableData(UserInfoTable.LastMap).Value;

        currentMapIdx.Value = Mathf.Max(lastIdx, 0);

        CurrentStageData = TableManager.Instance.stageMap.dataArray[currentMapIdx.Value];
    }

    public List<EnemyTableData> GetEnemyTableData()
    {
        List<EnemyTableData> enemyDatas = new List<EnemyTableData>();

        for (int i = 0; i < CurrentStageData.Spawnenemies.Length; i++)
        {
            enemyDatas.Add(TableManager.Instance.enemyTable.dataArray[CurrentStageData.Spawnenemies[i]]);
        }

        return enemyDatas;
    }

    public void LoadBackScene()
    {
        if (IsFirstScene() == false)
        {
            currentMapIdx.Value--;

            if (currentMapIdx.Value < 0)
            {
                currentMapIdx.Value = 0;
            }

            LoadNormalField();
        }
        else
        {
            PopupManager.Instance.ShowAlarmMessage("첫번째 스테이지 입니다.");
        }
    }

    private bool CanMoveStage = true;
    
    public void LoadNextScene()
    {
        if (IsLastScene() == false && CanMoveStage)
        {
            CanMoveStage = false;

            currentMapIdx.Value++;

            LoadNormalField();
        }
    }

    public void MoveMapByIdx(int idx)
    {
        currentMapIdx.Value = idx;
        LoadNormalField();
    }

    private Coroutine internetConnectCheckRoutine;


    public void LoadNormalField()
    {
        if (internetConnectCheckRoutine != null)
        {
            StopCoroutine(internetConnectCheckRoutine);
        }

        internetConnectCheckRoutine = StartCoroutine(checkInternetConnection((isConnected) =>
        {
            if (isConnected)
            {
                contentsType = ContentsType.NormalField;

                ClearStage();

                ChangeScene();
            }
            else
            {
                currentMapIdx.Value--;

                if (currentMapIdx.Value < 0)
                {
                    currentMapIdx.Value = 0;
                }

                PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, "네트워크가 불안정 합니다.\n잠시 후에 다시 시도해주세요.", null);
            }

            CanMoveStage = true;
        }));
    }

    IEnumerator checkInternetConnection(Action<bool> action)
    {
        action(true);
        yield break;

        WWW www = new WWW("http://google.com");
        yield return www;
        if (www.error != null)
        {
            action(false);
        }
        else
        {
            action(true);
        }
    }

    public void LoadContents(ContentsType type)
    {
        contentsType = type;
        
        ChangeScene();
    }
    private static bool firstLoad = true;
    
    private void ChangeScene()
    {
        //PopupManager.Instance.SetChatBoardPopupManager();

        if (firstLoad)
        {
            firstLoad = false;
            //PostManager.Instance.RefreshPost();
        }

        whenSceneChanged.Execute();

        UnityEngine.SceneManagement.SceneManager.LoadScene(1);


    }
    public bool IsLastScene()
    {
        return currentMapIdx.Value == TableManager.Instance.stageMap.dataArray.Length - 1;
    }
    public bool IsFirstScene()
    {
        return currentMapIdx.Value == 0;
    }

}
