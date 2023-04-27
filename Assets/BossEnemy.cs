using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class BossEnemy : MonoBehaviour
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
            //WhenEnemyDead();
        }).AddTo(this);
        
        UiBossDamageIndicator.Instance.SetDefault();
        
        enemyHpController.whenEnemyDamaged.AsObservable().Subscribe(e =>
        {
            UiBossDamageIndicator.Instance.UpdateDescription(e);
            //WhenEnemyDead();
        }).AddTo(this);
    }
    
    public void Initialize(EnemyInfo enemyInfo, EnemyType enemyType)
    {
        // //여기 여러번 타서 구독 여러번 하면 안됨
        // this.enemyInfo = enemyInfo;
        //
        // isFieldBossEnemy = isBossEnemy;
        
        enemyMoveController.Initialize(enemyInfo.MoveSpeed);

        enemyHpController.Initialize(enemyInfo,enemyType);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
