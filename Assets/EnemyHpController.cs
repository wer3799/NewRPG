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

    private EnemyInfo enemyInfo;

    [SerializeField]
    private EnemyHpBar enemyHpBar;

    private bool isEnemyDead = false;

    private Transform playerPos;

    private float defense;
    public float Defense => defense;

    [SerializeField]
    private Transform damTextSpawnPos;

    private bool initialized = false;

    private bool isBossEnemy = false;
 

    private void BaseInit()
    {
        if (initialized == false)
        {
            playerPos = PlayerMoveController.Instance.transform;
            
            Subscribe();
            
            initialized = true;
        }
    }

    private void Subscribe()
    {
        currentHp.AsObservable().Subscribe(e =>
        {
            if (isBossEnemy==false)
            {
                enemyHpBar.UpdateGauge(e, maxHp);
            }
            else
            {
                UiSubHpBar.Instance.UpdateGauge(e, maxHp);
            }

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

    public void Initialize(EnemyInfo enemyInfo,bool isBossEnemy=false)
    {
        this.enemyInfo = enemyInfo;

        this.isBossEnemy = isBossEnemy;
        
        BaseInit();

        SetDefense(enemyInfo.Defense);
        
        SetHp(enemyInfo.Hp);

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


    private Vector3 damTextspawnPos;
   
    
    public void SpawnDamText(bool isCritical, bool isSuperCritical, double value)
    {
        Vector3 spawnPos = Vector3.zero;

        if (damTextSpawnPos != null)
        {
            spawnPos = damTextSpawnPos.position;
        }
        else
        {
            spawnPos = this.transform.position;
        }



        damTextspawnPos = this.transform.position + Vector3.up*3;

      

        if (Vector3.Distance(playerPos.position, this.transform.position) < GameBalance.effectActiveDistance)
        {
            DamTextType damType = DamTextType.Normal;

            if (isCritical)
            {
                damType = DamTextType.Critical;
            }

            if (isSuperCritical)
            {
                damType = DamTextType.SuperCritical;
            }

            BattleObjectManagerAllTime.Instance.SpawnDamageText(value, damTextspawnPos, damType);
        }
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

        //this.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        ResetEnemy();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UpdateHp(-30f);

        }
        
    }
}
