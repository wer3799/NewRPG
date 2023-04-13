using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiGoldAbilBoard : MonoBehaviour
{
    [SerializeField]
    private GoldAbilStatusCell goldAbilStatusCell;

    [SerializeField]
    private Transform cellParent;

    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        var tableData = TableManager.Instance.goldAbilTable.dataArray;

        for (int i = 0; i < tableData.Length; i++)
        {
            var cell = Instantiate<GoldAbilStatusCell>(goldAbilStatusCell, cellParent);
            
            cell.Initialize(tableData[i]);
        }

    }
}
