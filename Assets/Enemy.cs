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

    private EnemyTableData enemyTableData;

    public bool isFieldBossEnemy { get; private set; } = false;

    private Action<Enemy> returnCallBack;

    public void SetReturnCallBack(Action<Enemy> returnCallBack)
    {
        this.returnCallBack = returnCallBack;
    }

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
            WhenEnemyDead();
        }).AddTo(this);
    }

    private void WhenEnemyDead()
    {
        this.gameObject.SetActive(false);

        if (isFieldBossEnemy)
        {
            NormalStageController.Instance.SetStageBossClear();
        }
            
        GrowthManager.Instance.GetExp(enemyTableData.Exp);

        ServerData.goodsTable.GetGoldByEnemy(enemyTableData.Gold);
        
        ServerData.goodsTable.GetGrowthStoneByEnemy(enemyTableData.Growthstone);
        
    }

    public void Initialize(EnemyTableData enemyTableData, bool isBossEnemy = false)
    {
        //여기 여러번 타서 구독 여러번 하면 안됨
        this.enemyTableData = enemyTableData;

        isFieldBossEnemy = isBossEnemy;
        
        enemyMoveController.Initialize(enemyTableData);

        enemyHpController.Initialize(enemyTableData,isBossEnemy);
    }

    private void OnDisable()
    {
        base.OnDisable();
        this.returnCallBack?.Invoke(this);

        // if (isFieldBossEnemy && MapInfo.Instance != null)
        // {
        //     MapInfo.Instance.SetCanSpawnEnemy(true);
        // }
    }
}