using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


public struct EnemyInfo
{
    public EnemyInfo(double Hp,float Exp,float Gold,float GrowthStone,float MoveSpeed,float Defense)
    {
        this.Hp = Hp;
        this.Exp = Exp;
        this.Gold = Gold;
        this.GrowthStone = GrowthStone;
        this.MoveSpeed = MoveSpeed;
        this.Defense = Defense;
    }
    
    public double Hp;
    public float Exp;
    public float Gold;
    public float GrowthStone;
    public float MoveSpeed;
    public float Defense;
}
public class Enemy : PoolItem
{
    [SerializeField]
    private EnemyMoveController enemyMoveController;

    [SerializeField]
    private EnemyHpController enemyHpController;

    private EnemyInfo enemyInfo;

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
            
        GrowthManager.Instance.GetExp(enemyInfo.Exp);

        ServerData.goodsTable.GetGoldByEnemy(enemyInfo.Gold);
        
        ServerData.goodsTable.GetGrowthStoneByEnemy(enemyInfo.GrowthStone);
        
    }

    public void Initialize(EnemyInfo enemyInfo, bool isBossEnemy = false)
    {
        //여기 여러번 타서 구독 여러번 하면 안됨
        this.enemyInfo = enemyInfo;

        isFieldBossEnemy = isBossEnemy;
        
        enemyMoveController.Initialize(enemyInfo.MoveSpeed);

        enemyHpController.Initialize(enemyInfo,isBossEnemy);
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