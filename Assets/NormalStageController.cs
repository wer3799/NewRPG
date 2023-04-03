using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class NormalStageController : SingletonMono<NormalStageController>
{
    public enum NormalStageState
    {
        Normal,Boss
    }
    
    private GameObject currentMapObject;

    private NormalStageState normalStageState = NormalStageState.Normal;

    private List<ObjectPool<Enemy>> spawnedEnemyList = new List<ObjectPool<Enemy>>();

    private Coroutine normalEnemySpawnRoutine;

    private StageMapData mapTableData;
    
    private void Start()
    {
        MakeStage();
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
    }
    
    private IEnumerator NormalEnemySpawnRoutine()
    {
        WaitForSeconds spawnDelay = new WaitForSeconds(mapTableData.Nextspawndelay);
        
        while (true)
        {
            for (int i = 0; i < mapTableData.Spawnenemies.Length; i++)
            {
                for (int j = 0; j < mapTableData.Spawnnum; j++)
                {
                    int randIdx = Random.Range(0, spawnedEnemyList.Count);

                    var enemyTableData = TableManager.Instance.EnemyData[mapTableData.Spawnenemies[i]];

                    var enemyPrefab = spawnedEnemyList[randIdx].GetItem();
                
                    enemyPrefab.Initialize(enemyTableData);
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
        
    }

    private void MakeObjectByCurrentStageId()
    {
        int currentStageId = (int)ServerData.userInfoTable.TableDatas[UserInfoTable.CurrentStage].Value;
        
        mapTableData = TableManager.Instance.stageMap.dataArray[currentStageId];

        var mapPrefab = Resources.Load<GameObject>($"Map/{mapTableData.Mappreset}");

        currentMapObject = GameObject.Instantiate(mapPrefab);
        currentMapObject.transform.localPosition = Vector3.zero;

        List<string> enemyPrefabNames = mapTableData.Spawnenemies.ToList();

        for (int i = 0; i < enemyPrefabNames.Count; i++)
        {
            spawnedEnemyList.Add(new ObjectPool<Enemy>(Resources.Load<Enemy>($"Enemy/{enemyPrefabNames[i]}"),this.transform,10));
        }
    }
}