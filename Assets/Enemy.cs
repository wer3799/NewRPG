using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : PoolItem
{
    [SerializeField]
    private EnemyMoveController enemyMoveController;

    [SerializeField]
    private EnemyHpController enemyHpController;

#if UNITY_EDITOR
    private void OnValidate()
    {
        enemyMoveController = GetComponent<EnemyMoveController>();
        enemyHpController = GetComponent<EnemyHpController>();
    }
#endif

    public void Initialize(EnemyTableData enemyTableData)
    {
        if (enemyMoveController != null)
        {
            enemyMoveController.Initialize(enemyTableData);
        }

        if (enemyHpController != null)
        {
            enemyHpController.Initialize(enemyTableData);
        }
    }
}