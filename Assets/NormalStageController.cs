using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class NormalStageController : SingletonMono<NormalStageController>
{
    private GameObject currentMapObject;

    private List<GameObject> spawnedEnemyList = new List<GameObject>();

    private void Start()
    {
        MakeStage();
    }

    private void MakeStage()
    {
        DestroyPrefObjects();

        MakeObjectByCurrentStageId();
    }

    private void DestroyPrefObjects()
    {
        //맵제거
        if (currentMapObject != null)
        {
            Object.Destroy(currentMapObject);
        }

        //소환된 몬스터들 제거
        foreach (GameObject e in spawnedEnemyList)
        {
            Object.Destroy(e.gameObject);
        }

        spawnedEnemyList.Clear();
    }

    private void MakeObjectByCurrentStageId()
    {
        int currentStageId = (int)ServerData.userInfoTable.TableDatas[UserInfoTable.CurrentStage].Value;
        var mapTableData = TableManager.Instance.stageMap.dataArray[currentStageId];

        var mapPrefab = Resources.Load<GameObject>($"Map/{mapTableData.Mappreset}");

        currentMapObject = GameObject.Instantiate(mapPrefab);
        currentMapObject.transform.localPosition = Vector3.zero;
    }
}