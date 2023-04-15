using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UiGoodsView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI goodsAmount;

    [SerializeField]
    private Image goodsIcon;

    [SerializeField]
    private GoodsEnum goodsEnum;

    private void Start()
    {
        SetGoodsIcon();

        Subscribe();
    }

    private void SetGoodsIcon()
    {
        goodsIcon.sprite = ResourcesContainer.goodsContainter[goodsEnum];
    }

    private void Subscribe()
    {
        ServerData.goodsTable.GetTableData(goodsEnum).AsObservable().Subscribe(e =>
        {
            if (this.gameObject.activeInHierarchy)
            {
                UpdateUi();
            }
        }).AddTo(this);
    }

    private void UpdateUi()
    {
        goodsAmount.SetText(Utils.ConvertBigNum(ServerData.goodsTable.GetTableData(goodsEnum).Value));
    }

    private void OnEnable()
    {
        UpdateUi();
    }
}