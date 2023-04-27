using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class DamageTestManager : TimeOutContentsBase
{
    [SerializeField]
    private BossEnemy bossPrefab;

    private BossEnemy spawnedBossEnemy;

    private void Start()
    {
        base.Start();

        Subscribe();

        SpawnBoss();
    }

    private void Subscribe()
    {
        PlayerStatusController.Instance.whenPlayerDead.AsObservable().Subscribe(e => { EndContents(); }).AddTo(this);
    }

    private void SpawnBoss()
    {
        EnemyInfo enemyInfo = new EnemyInfo();
        enemyInfo.MoveSpeed = 1;
        enemyInfo.Defense = 0;
        
        spawnedBossEnemy = Instantiate<BossEnemy>(bossPrefab, this.transform);

        spawnedBossEnemy.Initialize(enemyInfo, EnemyType.UndeadBoss);

        //공격대상으로 지정
        AutoManager.Instance.ResetTarget();
        AutoManager.Instance.SetAutoTarget(spawnedBossEnemy.transform);

        spawnedBossEnemy.damagedAccum.AsObservable().Subscribe(e => { UiBossDamageIndicator.Instance.UpdateDescription(e); }).AddTo(this);
    }

    public override void WhenTimerEnd(Unit unit)
    {
        EndContents();
    }

    public void EndContents()
    {
        HideBossEnemy();

        RecordScore();

        ShowResultPopup();

        StartCoroutine(ExitRoutine());
    }

    public void HideBossEnemy()
    {
        spawnedBossEnemy.gameObject.SetActive(false);
    }

    public void RecordScore()
    {
        double currentScore = spawnedBossEnemy.damagedAccum.Value;

        double prefScore = ServerData.clearInfoServerTable.TableDatas[ContentsName.DamageTest.ToString()].Value;

        if (currentScore > prefScore)
        {
            ServerData.clearInfoServerTable.UpData(ContentsName.DamageTest.ToString(), currentScore);
        }
    }

    public void ShowResultPopup()
    {
        double currentScore = spawnedBossEnemy.damagedAccum.Value ;

        string scoreDescription = $"점수 : {Utils.ConvertBigNum(currentScore)} 기록!!!!";
        
        Debug.LogError(scoreDescription);
        
        PopupManager.Instance.ShowAlarmMessage(scoreDescription);
    }
}