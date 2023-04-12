using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GoodsEnumMaker : MonoBehaviour
{
    [SerializeField]
    private Goods goodsTableData;

    [SerializeField]
    private int generate;

#if UNITY_EDITOR

    private void OnValidate()
    {
        string str = string.Empty;

        var tableData = goodsTableData.dataArray;

        for (int i = 0; i < tableData.Length; i++)
        {
            str += tableData[i].Namekey;

            if (i != tableData.Length - 1)
            {
                str += ",";
            }
        }

        EnumCodeGenerator.GenerateEnumFromTextFile(str.Split(','), "GoodsEnum", Application.dataPath +"/GoodsEnum.cs");

        Debug.LogError(str);
    }

#endif
}