using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class DamageTestManager : TimeOutContentsBase
{
    [SerializeField]
    private BossEnemy bossPrefab;

    private void Start()
    {
        base.Start();

        SpawnBoss();
    }

    private void SpawnBoss()
    {
        var bossEnemy = Instantiate<BossEnemy>(bossPrefab, this.transform);

        EnemyInfo enemyInfo = new EnemyInfo();
        enemyInfo.MoveSpeed = 1;
        enemyInfo.Defense = 0;
        
        bossEnemy.Initialize(enemyInfo,EnemyType.UndeadBoss);
        
        AutoManager.Instance.ResetTarget();
        AutoManager.Instance.SetAutoTarget(bossEnemy.transform);
    }

    public override void WhenTimerEnd(Unit unit)
    {
        //결과팝업?

        //나가기
        ContentsMakeController.Instance.ExitCurrentContents();
    }
}