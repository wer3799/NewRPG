using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class NormalStageMap : MonoBehaviour
{
    [SerializeField]
    private Transform spawnedPointRoot;

    [SerializeField]
    private Transform bossSpawnPos;

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

    public Vector3 GetBossSpawnPos()
    {
        if (bossSpawnPos == null)
        {
            return Vector3.zero;
        }
        else
        {
            return bossSpawnPos.position;
        }
    }
}