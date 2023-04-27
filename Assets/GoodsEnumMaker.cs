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
    private MainContents mainContentsTableData;
    
    public void MakeGoodsEnum()
    {
        string str = string.Empty;

        var tableData = goodsTableData.dataArray;

        for (int i = 0; i < tableData.Length; i++)
        {
            str += tableData[i].Stringid+$"={tableData[i].Id}";

            if (i != tableData.Length - 1)
            {
                str += ",";
            }
        }

        EnumCodeGenerator.GenerateEnumFromTextFile(str.Split(','), "GoodsEnum", Application.dataPath +"/GoodsEnum.cs");

        Debug.LogError(str);
    }
    
    public void MakeMainContentsEnum()
    {
        string str = string.Empty;

        var tableData = mainContentsTableData.dataArray;

        for (int i = 0; i < tableData.Length; i++)
        {
            str += tableData[i].Contentsname+$"={tableData[i].Id}";;

            if (i != tableData.Length - 1)
            {
                str += ",";
            }
        }

        EnumCodeGenerator.GenerateEnumFromTextFile(str.Split(','), "ContentsName", Application.dataPath +"/ContentsName.cs");

        Debug.LogError(str);
    }

}