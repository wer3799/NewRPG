using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class NormalStageController : SingletonMono<NormalStageController>
{
    public enum NormalStageState
    {
        Normal,
        Boss
    }

    private GameObject currentMapObject;

    private ReactiveProperty<NormalStageState> stageState = new ReactiveProperty<NormalStageState>();

    public NormalStageState StageState => stageState.Value;

    private List<ObjectPool<Enemy>> spawnedEnemyList = new List<ObjectPool<Enemy>>();

    public List<Enemy> currentSpawnedEnemies = new List<Enemy>();

    private Coroutine normalEnemySpawnRoutine;

    private ReactiveProperty<StageMapData> mapTableData = new ReactiveProperty<StageMapData>();

    public ReactiveProperty<StageMapData> MapTableData => mapTableData;

    private NormalStageMap normalStageMap;


    public bool IsBossState => stageState.Value == NormalStageState.Boss;

    private void Start()
    {
        MakeStage();

        Subscribe();
    }

    private void Subscribe()
    {
        disposable.Clear();

        stageState.AsObservable().Subscribe(e =>
        {
            if (e == NormalStageState.Normal)
            {
                disposable.Clear();
            }
        }).AddTo(this);
    }


    private void MakeStage()
    {
        DestroyPrefObjects();

        MakeObjectByCurrentStageId();

        if (normalEnemySpawnRoutine != null)
        {
            StopCoroutine(normalEnemySpawnRoutine);
        }

        normalEnemySpawnRoutine = StartCoroutine(NormalEnemySpawnRoutine());

        AutoManager.Instance.SetAuto(true);
    }

    private IEnumerator NormalEnemySpawnRoutine()
    {
        WaitForSeconds spawnDelay = new WaitForSeconds(mapTableData.Value.Nextspawndelay);

        while (true)
        {
            for (int i = 0; i < mapTableData.Value.Spawnenemies.Length; i++)
            {
                for (int j = 0; j < mapTableData.Value.Spawnnum; j++)
                {
                    if (stageState.Value == NormalStageState.Boss) continue;

                    int randIdx = Random.Range(0, spawnedEnemyList.Count);

                    var enemyTableData = TableManager.Instance.EnemyData[mapTableData.Value.Spawnenemies[i]];

                    var enemyPrefab = spawnedEnemyList[randIdx].GetItem();

                    enemyPrefab.Initialize(enemyTableData);

                    enemyPrefab.transform.localScale = Vector3.one;

                    enemyPrefab.SetReturnCallBack(EnemyRemoveCallBack);

                    enemyPrefab.transform.position = normalStageMap.GetRandomSpawnPos();

                    currentSpawnedEnemies.Add(enemyPrefab);
                }
            }

            yield return spawnDelay;
        }
    }

    private void DestroyPrefObjects()
    {
        //맵제거
        if (currentMapObject != null)
        {
            Object.Destroy(currentMapObject);
        }

        //소환된 몬스터들 제거
        for (int i = 0; i < spawnedEnemyList.Count; i++)
        {
            spawnedEnemyList[i].DestroyAllItems();
            spawnedEnemyList[i] = null;
        }

        spawnedEnemyList.Clear();

        currentSpawnedEnemies.Clear();
    }


    private void MakeObjectByCurrentStageId()
    {
        int currentStageId = (int)ServerData.userInfoTable.TableDatas[UserInfoTable.CurrentStage].Value;

        mapTableData.Value = TableManager.Instance.stageMap.dataArray[currentStageId];

        var mapPrefab = Resources.Load<GameObject>($"Map/{mapTableData.Value.Mappreset}");

        currentMapObject = GameObject.Instantiate(mapPrefab);
        currentMapObject.transform.localPosition = Vector3.zero;
        normalStageMap = currentMapObject.GetComponent<NormalStageMap>();

        List<string> enemyPrefabNames = mapTableData.Value.Spawnenemies.ToList();

        for (int i = 0; i < enemyPrefabNames.Count; i++)
        {
            spawnedEnemyList.Add(new ObjectPool<Enemy>(Resources.Load<Enemy>($"Enemy/{enemyPrefabNames[i]}"), this.transform, 10));
        }
    }

    private void EnemyRemoveCallBack(Enemy enemy)
    {
        currentSpawnedEnemies.Remove(enemy);
    }

    private CompositeDisposable disposable = new CompositeDisposable();
    
    public void StartStageBoss()
    {
        stageState.Value = NormalStageState.Boss;
        
        DisableAllEnemies();
        
        SpawnBossEnemy();
        
        UiSubHpBar.Instance.ShowGauge(true);
        
        UiBossTimer.Instance.StartBossTimer(GameBalance.stageBossClearTime);
        
        disposable.Clear();

        UiBossTimer.Instance.whenFieldBossTimerEnd.AsObservable().Subscribe(e =>
        {
            
            StageBossTimeOut();

        }).AddTo(disposable);
    }

    private void SpawnBossEnemy()
    {
        var enemyTableData = TableManager.Instance.EnemyData[mapTableData.Value.Spawnenemies[0]];

        var enemyPrefab = spawnedEnemyList[0].GetItem();

        enemyPrefab.Initialize(enemyTableData, isBossEnemy: true);

        enemyPrefab.SetReturnCallBack(EnemyRemoveCallBack);

        enemyPrefab.transform.position = normalStageMap.GetBossSpawnPos();

        enemyPrefab.transform.localScale = Vector3.one * 5f;

        currentSpawnedEnemies.Add(enemyPrefab);
    }

    private void DisableAllEnemies()
    {
        for (int i = 0; i < spawnedEnemyList.Count; i++)
        {
            spawnedEnemyList[i].DisableAllObject();
        }
    }

    public void MoveNextStage()
    {
        PopupManager.Instance.ShowStageChangeEffect();
        
        DestroyPrefObjects();

        MakeStage();
    }

    public void StageBossTimeOut()
    {
        //보스 끄기
        for (int i = 0; i < currentSpawnedEnemies.Count; i++)
        {
            currentSpawnedEnemies[i].gameObject.SetActive(false);
        }
        
        stageState.Value = NormalStageState.Normal;
        
        UiSubHpBar.Instance.ShowGauge(false);

        UiBossTimer.Instance.StopBossTimer();
    }

    public void SetStageBossClear()
    {
        stageState.Value = NormalStageState.Normal;
        
        UiSubHpBar.Instance.ShowGauge(false);
        
        UiBossTimer.Instance.StopBossTimer();
        
        ServerData.userInfoTable.TableDatas[UserInfoTable.CurrentStage].Value++;

        if (stageSyncRoutine != null)
        {
            StopCoroutine(stageSyncRoutine);
        }

        StartCoroutine(StageSyncRoutine());
    }

    private WaitForSeconds delay = new WaitForSeconds(10.0f);
    private Coroutine stageSyncRoutine;

    private IEnumerator StageSyncRoutine()
    {
        yield return delay;

        ServerData.userInfoTable.UpData(UserInfoTable.CurrentStage, false);
    }

    private void OnDestroy()
    {
        base.OnDestroy();
        disposable.Dispose();
    }
    
}