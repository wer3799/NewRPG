using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NormalStageMap : MonoBehaviour
{
    [SerializeField]
    private Transform spawnedPointRoot;

    [SerializeField]
    private List<Transform> spawnedList;

    #if UNITY_EDITOR
    private void OnValidate()
    {
        spawnedList.Clear();

        var spawnedPoints = spawnedPointRoot.GetComponentsInChildren<Transform>(spawnedPointRoot);

        spawnedList = spawnedPoints.ToList();
        spawnedList.RemoveAt(0);
    }
    #endif

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
