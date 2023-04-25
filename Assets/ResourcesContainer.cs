using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ResourcesContainer
{
    public static Dictionary<GoodsEnum, Sprite> goodsContainter = new Dictionary<GoodsEnum, Sprite>();

    public static void Initialize()
    {
        var goodsTableData = TableManager.Instance.goodsTable.dataArray;

        for (int i = 0; i < goodsTableData.Length; i++)
        {
            goodsContainter.Add(Enum.Parse<GoodsEnum>(goodsTableData[i].Stringid),Resources.Load<Sprite>(goodsTableData[i].Stringid));
        }
    }


}
