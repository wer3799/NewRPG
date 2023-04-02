using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveController : MonoBehaviour
{
    private Transform playerTr;

    [SerializeField]
    private Rigidbody2D rb;

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

    public void Initialize(EnemyTableData enemyTableData)
    {

    }
}