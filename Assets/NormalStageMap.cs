using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class NormalStageMap : MonoBehaviour
{
    [SerializeField]
    private Transform spawnedPointRoot;

    [SerializeField]
    public List<Transform> spawnedList;

    #if UNITY_EDITOR
    private void OnValidate()
    {
        spawnedList.Clear();

        var spawnedPoints = spawnedPointRoot.GetComponentsInChildren<Transform>(spawnedPointRoot);

        spawnedList = spawnedPoints.ToList();
        spawnedList.RemoveAt(0);
    }
    #endif

    public Vector3 GetRandomSpawnPos()
    {
        return spawnedList[Random.Range(0, spawnedList.Count)].transform.position;
    }
}
