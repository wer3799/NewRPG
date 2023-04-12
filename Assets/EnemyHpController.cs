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

    public void Initialize(EnemyTableData enemyTableData,bool isBossEnemy=false)
    {
        this.enemyTableData = enemyTableData;

        this.isBossEnemy = isBossEnemy;
        
        BaseInit();

        SetDefense(enemyTableData.Defense);
        
        SetHp(enemyTableData.Hp * enemyTableData.Bosshpratio);

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
    private Coroutine damTextRoutine;
    private int attackCount = 0;
    private int attackCountMax = 16;
    private double attackResetCount = 0f;
    private double damTextMergeTime = 0.5f;
    private const float damTextCountAddValue = 0.1f;
    private readonly WaitForSeconds DamTextDelay = new WaitForSeconds(damTextCountAddValue);
    private float rightValue = 0f;
    private float upValue = 2f;
    
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

        if (damTextRoutine == null && isEnemyDead == false)
        {
            damTextRoutine = StartCoroutine(DamTextRoutine());
        }

        damTextspawnPos = this.transform.position + Vector3.up * attackCount * 1f + Vector3.right * rightValue + Vector3.up * upValue;

        attackCount++;

        if (attackCount == attackCountMax)
        {
            attackCount = 0;
            rightValue = UnityEngine.Random.Range(-3f, 3f);
            upValue = UnityEngine.Random.Range(2f, 4.5f);
        }

        attackResetCount = 0f;

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
    
    private IEnumerator DamTextRoutine()
    {
        while (attackResetCount < damTextMergeTime)
        {
            yield return DamTextDelay;
            attackResetCount += 0.1f;
        }

        ResetDamTextValue();
    }
    private void ResetDamTextValue()
    {
        attackCount = 0;
        attackResetCount = 0f;
        damTextRoutine = null;
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
