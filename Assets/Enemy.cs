using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
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

    private void Awake()
    {
        enemyHpController.whenEnemyDead.AsObservable().Subscribe(e =>
        {
            this.gameObject.SetActive(false);

            ReturnToPool();
        }).AddTo(this);
    }

    public void Initialize(EnemyTableData enemyTableData)
    {
        //여기 여러번 타서 구독 여러번 하면 안됨

        enemyMoveController.Initialize(enemyTableData);

        enemyHpController.Initialize(enemyTableData);
    }
}