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

    private CompositeDisposable bossTimerDisposable = new CompositeDisposable();

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
        bossTimerDisposable.Clear();

        stageState.AsObservable().Subscribe(e =>
        {
            if (e == NormalStageState.Normal)
            {
                bossTimerDisposable.Clear();
            }
        }).AddTo(this);
    }


    public void MakeStage()
    {
        DestroyPrefObjects();

        MakeObjectByCurrentStageId();

        StartSpanwRoutine();

        AutoManager.Instance.SetAuto(true);
    }

    public void DisableStage()
    {
        StopSpawnRoutine();
        DestroyPrefObjects();
    }

    public void StopSpawnRoutine()
    {
        if (normalEnemySpawnRoutine != null)
        {
            StopCoroutine(normalEnemySpawnRoutine);
        }
    }

    public void StartSpanwRoutine()
    {
        StopSpawnRoutine();

        normalEnemySpawnRoutine = StartCoroutine(NormalEnemySpawnRoutine());
    }

    public EnemyInfo GetCurrentStageEnemyInfo(bool bossEnemy = false)
    {
        EnemyTableData enemyTableData = TableManager.Instance.GetTableDataByLevel(MapTableData.Value.Monsterlevel);

        double Hp = enemyTableData.Starthp + enemyTableData.Perhp * MapTableData.Value.Monsterlevel;
        float Exp = MapTableData.Value.Exp;
        float Gold = MapTableData.Value.Gold;
        float GrowthStone = MapTableData.Value.Growthstoneamount;
        float MoveSpeed = MapTableData.Value.Movespeed;
        float Defense = enemyTableData.Startdef * enemyTableData.Intervaldef;

        if (bossEnemy)
        {
            Hp *= MapTableData.Value.Multiplebosspower;
            Defense *= (float)MapTableData.Value.Multiplebosspower;
        }

        return new EnemyInfo(Hp, Exp, Gold, GrowthStone, MoveSpeed, Defense);
    }

    private IEnumerator NormalEnemySpawnRoutine()
    {
        WaitForSeconds spawnCheckDelay = new WaitForSeconds(mapTableData.Value.Spawndelay);

        while (true)
        {
            EnemyInfo enemyInfo = GetCurrentStageEnemyInfo();

            while (currentSpawnedEnemies.Count < mapTableData.Value.Spawnamount)
            {
                if (stageState.Value == NormalStageState.Boss)
                {
                    yield return null;
                    continue;
                }

                int randIdx = Random.Range(0, spawnedEnemyList.Count);

                var enemyPrefab = spawnedEnemyList[randIdx].GetItem();

                enemyPrefab.Initialize(enemyInfo,EnemyType.Normal);

                enemyPrefab.transform.localScale = Vector3.one;

                enemyPrefab.SetReturnCallBack(EnemyRemoveCallBack);

                enemyPrefab.transform.position = normalStageMap.GetRandomSpawnPos();

                currentSpawnedEnemies.Add(enemyPrefab);
            }
            
            yield return spawnCheckDelay;
        }
    }

    public void DestroyPrefObjects()
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
        int currentStageId = (int)ServerData.clearInfoServerTable.TableDatas[ContentsName.NormalField.ToString()].Value;

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


    public void StartStageBoss()
    {
        stageState.Value = NormalStageState.Boss;

        DisableAllEnemies();

        SpawnBossEnemy();

        UiSubHpBar.Instance.ShowGauge(true);

        UiBossTimer.Instance.StartBossTimer(GameBalance.stageBossClearTime);

        bossTimerDisposable.Clear();

        UiBossTimer.Instance.whenFieldBossTimerEnd.AsObservable().Subscribe(e => { StageBossTimeOut(); }).AddTo(bossTimerDisposable);
    }

    private void SpawnBossEnemy()
    {
        EnemyInfo enemyInfo = GetCurrentStageEnemyInfo(true);

        var enemyPrefab = spawnedEnemyList[0].GetItem();

        enemyPrefab.Initialize(enemyInfo, EnemyType.StageBoss);

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

        ServerData.clearInfoServerTable.TableDatas[ContentsName.NormalField.ToString()].Value++;

        if (stageSyncRoutine != null)
        {
            StopCoroutine(stageSyncRoutine);
        }

        StartCoroutine(StageSyncRoutine());
    }

    private WaitForSeconds delay = new WaitForSeconds(1.0f);
    private Coroutine stageSyncRoutine;

    private IEnumerator StageSyncRoutine()
    {
        yield return delay;
        
        ServerData.clearInfoServerTable.UpData(ContentsName.NormalField.ToString(), ServerData.clearInfoServerTable.TableDatas[ContentsName.NormalField.ToString()].Value);
    }

    private void OnDestroy()
    {
        base.OnDestroy();
        bossTimerDisposable.Dispose();
    }
}