using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class EnemyHpController : MonoBehaviour
{
 private ReactiveProperty<double> currentHp = new ReactiveProperty<double>();
    public double CurrentHp => currentHp.Value;
    public ReactiveCommand whenEnemyDead { get; private set; } = new ReactiveCommand();
    public ReactiveCommand<double> whenEnemyDamaged { get; private set; } = new ReactiveCommand<double>();
    public double maxHp { get; private set; }

    private EnemyTableData enemyTableData;

    [SerializeField]
    private EnemyHpBar enemyHpBar;

    private bool isEnemyDead = false;

    private Transform playerPos;

    private float defense;
    public float Defense => defense;

    [SerializeField]
    private Transform damTextSpawnPos;

    private bool initialized = false;

    private void Start()
    {
        Subscribe();
        SetPlayerTr();
    }

    private void SetPlayerTr()
    {
        if (initialized == false)
        {
            playerPos = PlayerMoveController.Instance.transform;
            initialized = true;
        }
    }

    private void Subscribe()
    {
        currentHp.AsObservable().Subscribe(e =>
        {
            enemyHpBar.UpdateGauge(e, maxHp);
        }).AddTo(this);
    }

    private void ResetEnemy()
    {
        currentHp.Value = maxHp;
        isEnemyDead = false;
    }

    public void SetDefense(float defense)
    {
        this.defense = defense;
    }

    public void Initialize(EnemyTableData enemyTableData)
    {
        this.enemyTableData = enemyTableData;

        SetDefense(enemyTableData.Defense);
        
        SetHp(enemyTableData.Hp);
    }

    public void SetHp(double hp)
    {
        this.maxHp = hp;
        currentHp.Value = maxHp;
    }
   
    private static string hitSfxName = "EnemyHitted";
    private static string deadSfxName = "EnemyDead";
    
    public void ApplyPlusDamage(ref double value, bool isCritical, bool isSuperCritical)
    {
        
    }


    public void SpawnDamText(bool isCritical, bool isSuperCritical, double value)
    {
       
    }

    public void UpdateHp(double value)
    {
        if (isEnemyDead == true) return;

        if (value < 0)
        {
            currentHp.Value += value;
        }
        
        whenEnemyDamaged.Execute(value);

        if (currentHp.Value <= 0)
        {
            EnemyDead();
            return;
        }
    }
    private float ignoreDefenseValue;

    public void ApplyDefense(ref double value)
    {
        ignoreDefenseValue =0;

        float enemyDefense = Mathf.Max(0f, defense - ignoreDefenseValue);

        value -= value * enemyDefense * 0.01f;
    }

    private void EnemyDead()
    {
        whenEnemyDead.Execute();

        isEnemyDead = true;

        this.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        ResetEnemy();
    }
}
