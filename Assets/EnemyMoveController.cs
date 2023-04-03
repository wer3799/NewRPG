using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveController : MonoBehaviour
{
    private static WaitForSeconds setDirectionDelay = new WaitForSeconds(0.1f);
    
    public enum EnemyMoveType
    {
        Follow
    }
    
    private Transform playerTr;

    [SerializeField]
    private Rigidbody2D rb;

    [SerializeField]
    private EnemyMoveType enemyMoveType;

    private EnemyTableData enemyTableData;
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        rb = GetComponent<Rigidbody2D>();
    }
#endif

    void Start()
    {
        SetPlayerTr();
    }

    private void SetPlayerTr()
    {
        playerTr = PlayerMoveController.Instance.transform;
    }

    private IEnumerator FollowRoutine()
    {
        while (true)
        {
            Vector2 direction = playerTr.transform.position - this.transform.position;
            this.rb.velocity = direction.normalized * enemyTableData.Movespeed;
            yield return setDirectionDelay;
        }
        
    }

    public void Initialize(EnemyTableData enemyTableData)
    {
        this.enemyTableData = enemyTableData;

        StartMoveRoutine();
    }

    private void StartMoveRoutine()
    {
        switch (enemyMoveType)
        {
            case EnemyMoveType.Follow:
                StartCoroutine(FollowRoutine());
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }
}