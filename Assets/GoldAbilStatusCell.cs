using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using BackEnd;
using CodeStage.AntiCheat.ObscuredTypes;

public class GoldAbilStatusCell : MonoBehaviour
{
    [SerializeField]
    private Image statusIcon;

    [SerializeField]
    private TextMeshProUGUI levelText;

    [SerializeField]
    private TextMeshProUGUI statusNameText;

    [SerializeField]
    private TextMeshProUGUI descriptionText;

    [SerializeField]
    private Image upgradeButton;

    // [SerializeField]
    // private Image upgradeButton_100;
    //
    // [SerializeField]
    // private Image upgradeButton_10000;
    //
    // [SerializeField]
    // private Image upgradeButton_all; 

    [SerializeField]
    private Sprite enableSprite;

    [SerializeField]
    private Sprite disableSprite;

    [SerializeField]
    private Sprite maxLevelSprite;

    [SerializeField]
    private TextMeshProUGUI priceText;

    private int currentLevel = -1;

    private float upgradePrice_gold;

    private WaitForSeconds autuSaveDelay = new WaitForSeconds(1.0f);
    private WaitForSeconds autuUpFirstDelay = new WaitForSeconds(1.0f);
    private WaitForSeconds autuUpDelay = new WaitForSeconds(0.01f);

    private GoldAbilTableData goldAbilTable;

    [SerializeField]
    private TextMeshProUGUI upgradeText;

    [SerializeField]
    private GameObject lockMask;

    [SerializeField]
    private TextMeshProUGUI lockDescription;
    public void Initialize(GoldAbilTableData goldAbilTable)
    {
        this.goldAbilTable = goldAbilTable;

        Subscribe();

        SetUpgradeButtonState(CanUpgrade());

        if (IsMaxLevel())
        {
            upgradeText.SetText("최고단계");
        }
        else
        {
            upgradeText.SetText("수련");
        }

        //statusIcon.sprite = CommonUiContainer.Instance.statusIcon[statusData.Statustype];
    }

    private void RefreshStatusText()
    {
        float currentStatusValue = ServerData.goldAbilServerTable.GetStatusValue(goldAbilTable.Stringid, currentLevel);
        float nextStatusValue = ServerData.goldAbilServerTable.GetStatusValue(goldAbilTable.Stringid, currentLevel + 1);

        float price = 0f;

        price = ServerData.goldAbilServerTable.GetStatusUpgradePrice(goldAbilTable.Stringid, currentLevel);

        priceText.SetText(Utils.ConvertBigNum(price));

        upgradePrice_gold = price;

        statusNameText.SetText(GameString.GetString(goldAbilTable.Name));

        if (goldAbilTable.Ispercent == false)
        {
            if (IsMaxLevel() == false)
            {
                descriptionText.SetText($"{Utils.ConvertBigNum(currentStatusValue)}->{Utils.ConvertBigNum(nextStatusValue)}");
            }
            else
            {
                descriptionText.SetText($"{Utils.ConvertBigNum(currentStatusValue)}(MAX)");
            }
        }
        //%로 표시
        else
        {
            if (IsMaxLevel() == false)
            {
                descriptionText.SetText($"{Utils.ConvertBigNum(currentStatusValue * 100f)}%->{Utils.ConvertBigNum(nextStatusValue * 100f)}%");
            }
            else
            {
                descriptionText.SetText($"{Utils.ConvertBigNum(currentStatusValue * 100f)}%(MAX)");
            }
        }

        levelText.SetText($"Lv : {currentLevel}");
    }

    private void Subscribe()
    {
        ServerData.goldAbilServerTable.GetTableData(goldAbilTable.Stringid).AsObservable().Subscribe(currentLevel =>
        {
            this.currentLevel = currentLevel;
            RefreshStatusText();
        }).AddTo(this);

        ServerData.goodsTable.GetTableData(GoodsTable.GoodsKey[GoodsEnum.Gold]).AsObservable().Subscribe(e =>
        {
            if (this.gameObject.activeInHierarchy)
            {
                SetUpgradeButtonState(CanUpgrade());
            }
        }).AddTo(this);


        if (goldAbilTable.Unlocklevel != 0 && goldAbilTable.Needstatuskey != -1)
        {
            var localTableData = TableManager.Instance.goldAbilTable.dataArray[goldAbilTable.Needstatuskey];

            lockDescription.SetText($"{GameString.GetString(localTableData.Name)} LV : {goldAbilTable.Unlocklevel} 이상 필요");

            ServerData.goldAbilServerTable.GetTableData(localTableData.Stringid).AsObservable().Subscribe(e => { lockMask.SetActive(goldAbilTable.Unlocklevel >= e + 2); }).AddTo(this);
        }
        else
        {
            lockMask.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (goldAbilTable != null)
        {
            SetUpgradeButtonState(CanUpgrade());
        }
    }

    private Coroutine saveRoutine;
    private void SyncToServer()
    {
        if (saveRoutine != null)
        {
            CoroutineExecuter.Instance.StopCoroutine(saveRoutine);
        }

        saveRoutine = CoroutineExecuter.Instance.StartCoroutine(SaveRoutine());
    }

    public void OnClickButton()
    {
        if (CanUpgrade(true) == false)
        {
            return;
        }

        ServerData.goodsTable.GetTableData(GoodsTable.GoodsKey[GoodsEnum.Gold]).Value -= upgradePrice_gold;
        
        ServerData.goldAbilServerTable.GetTableData(goldAbilTable.Stringid).Value += 1;
        
        SyncToServer();
    }

    private bool IsMaxLevel()
    {
        return goldAbilTable.Maxlv <= ServerData.goldAbilServerTable.GetTableData(goldAbilTable.Stringid).Value;
    }

    private bool CanUpgrade(bool showPopup = false)
    {
        if (IsMaxLevel())
        {
            upgradeText.SetText("최고레벨");

            if (showPopup)
            {
                PopupManager.Instance.ShowAlarmMessage("최고레벨 입니다.");
            }

            return false;
        }

        bool ret = ServerData.goodsTable.GetTableData(GoodsTable.GoodsKey[GoodsEnum.Gold]).Value >= upgradePrice_gold;

        if (showPopup && ret == false)
        {
            PopupManager.Instance.ShowAlarmMessage($"XXX가 부족합니다.");
        }

        return ret;
    }

    private void DisableUpgradeButton()
    {
        SetUpgradeButtonState(false);
    }

    private void SetUpgradeButtonState(bool on)
    {
        if (upgradeButton == null) return;

        upgradeButton.raycastTarget = on;

        if (IsMaxLevel() == false)
        {
            upgradeButton.sprite = on ? enableSprite : disableSprite;
            // upgradeButton_100.sprite = on ? enableSprite : disableSprite;
            // upgradeButton_all.sprite = on ? enableSprite : disableSprite;
            // upgradeButton_10000.sprite = on ? enableSprite : disableSprite;
        }
        else
        {
            upgradeButton.sprite = maxLevelSprite;
            // upgradeButton_100.sprite = maxLevelSprite;
            // upgradeButton_all.sprite = maxLevelSprite;
            // upgradeButton_10000.sprite = maxLevelSprite;
        }
    }

    public void OnClickAllUpgradeButton()
    {
        //골드 기능구현필요

        //싱크
        SyncToServer();

        SetUpgradeButtonState(CanUpgrade());
    }

    public void OnClick100_Upgrade()
    {
        //골드 기능구현필요

        //싱크
        SyncToServer();

        SetUpgradeButtonState(CanUpgrade());
    }

    public void OnClick10000_Upgrade()
    {
        //골드 기능구현필요

        //싱크
        SyncToServer();

        SetUpgradeButtonState(CanUpgrade());
    }

    private IEnumerator SaveRoutine()
    {
        yield return autuSaveDelay;

        SyncData();

        saveRoutine = null;
    }

    private void SyncData()
    {
        List<TransactionValue> transactionList = new List<TransactionValue>();

        Param statusParam = new Param();
        Param goodesParam = new Param();

        //능력치
        statusParam.Add(goldAbilTable.Stringid, ServerData.goldAbilServerTable.GetTableData(goldAbilTable.Stringid).Value);

        //스킬포인트

        goodesParam.Add(GoodsTable.GoodsKey[GoodsEnum.Gold], ServerData.goodsTable.GetTableData(GoodsTable.GoodsKey[GoodsEnum.Gold]).Value);
        transactionList.Add(TransactionValue.SetUpdateV2(GoodsTable.tableName, GoodsTable.Indate, Backend.UserInDate, goodesParam));

        transactionList.Add(TransactionValue.SetUpdateV2(GoldAbilServerTable.tableName, GoldAbilServerTable.Indate, Backend.UserInDate, statusParam));

        ServerData.SendTransaction(transactionList);
    }
}