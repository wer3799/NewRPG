using BackEnd;
using CodeStage.AntiCheat.ObscuredTypes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UiGachaResultView;
using UniRx;
using UnityEngine.Serialization;

public class UiItemGacha : MonoBehaviour
{
    public ItemType itemType;

    public string GetGachaLevelKey()
    {
        return $"{UiGachaPopup.GachaNumKey}{(int)itemType}";
    }
    private List<ItemData> itemDatas = new List<ItemData>();
    private List<float> probs = new List<float>();
    private List<GachaResultCellInfo> gachaResultCellInfos = new List<GachaResultCellInfo>();

    [SerializeField]
    private List<ObscuredInt> gachaAmount;

    [SerializeField]
    private List<ObscuredInt> gachaPrice;

    private ObscuredInt lastGachaIdx = 0;

    [SerializeField]
    private List<TextMeshProUGUI> gachaNumTexts;

    [SerializeField]
    private List<TextMeshProUGUI> priceTexts;

    [SerializeField]
    private TextMeshProUGUI freeButtonDesc;

    private void Subscribe()
    {
        // ServerData.userInfoTable.GetTableData(UserInfoTable.freeWeapon).Subscribe(e =>
        // {
        //     freeButtonDesc.SetText(e == 0 ? "무료 뽑기!" : "내일 다시!");
        // }).AddTo(this);
    }

    private void Start()
    {
        Initialize();
        Subscribe();
    }

    private void Initialize()
    {
        for (int i = 0; i < gachaNumTexts.Count; i++)
        {
            gachaNumTexts[i].SetText($"{gachaAmount[i]}번 소환");
        }

        for (int i = 0; i < priceTexts.Count; i++)
        {
            priceTexts[i].SetText($"{gachaPrice[i]}");
        }
    }

    private bool CanGacha(float price)
    {
        float currentBlueStoneNum = ServerData.goodsTable.GetTableData(GoodsTable.GoodsKey[GoodsEnum.Diamond]).Value;
        return currentBlueStoneNum >= price;
    }

    public void OnClickFreeGacha()
    {
        // bool canFreeGacha = ServerData.userInfoTable.GetTableData(UserInfoTable.freeWeapon).Value == 0;
        //
        // if (canFreeGacha == false)
        // {
        //     PopupManager.Instance.ShowAlarmMessage("오늘은 더이상 받을 수 없습니다.");
        //     return;
        // }
        //
        // AdManager.Instance.ShowRewardedReward(() =>
        // {
        //     ServerData.userInfoTable.GetTableData(UserInfoTable.freeWeapon).Value = 1;
        //
        //     List<TransactionValue> transactions = new List<TransactionValue>();
        //
        //     Param userInfoParam = new Param();
        //
        //     userInfoParam.Add(UserInfoTable.freeWeapon, ServerData.userInfoTable.GetTableData(UserInfoTable.freeWeapon).Value);
        //
        //     transactions.Add(TransactionValue.SetUpdate(UserInfoTable.tableName, UserInfoTable.Indate, userInfoParam));
        //
        //     ServerData.SendTransaction(transactions, successCallBack: () =>
        //     {
        //         this.lastGachaIdx = 2;
        //         int amount = gachaAmount[2];
        //         int price = gachaPrice[2];
        //
        //         //무료라
        //         ServerData.goodsTable.GetTableData(GoodsTable.Jade).Value += price;
        //
        //         OnClickOpenButton(2);
        //
        //        // LogManager.Instance.SendLogType("FreeGacha", "Weapon", "");
        //     });
        //
        // });
    }

    public void OnClickOpenButton(int idx)
    {
        this.lastGachaIdx = idx;
        int amount = gachaAmount[idx];
        int price = gachaPrice[idx];

        //재화 체크
        if (CanGacha(price) == false)
        {
            PopupManager.Instance.ShowAlarmMessage($"미정이 부족합니다.");
            UiGachaResultView.Instance.autoToggle.isOn = false;
            return;
        }

        //UiTutorialManager.Instance.SetClear(TutorialStep.GetWeapon);
        
        gachaResultCellInfos.Clear();
        
        int gachaLevel = UiGachaPopup.Instance.GachaLevel(itemType);
        
        SetProbAndItemDatas(itemType,gachaLevel);
        
        List<int> serverUpdateList = new List<int>();

        //로컬 데이터 갱신

        //재화
        ServerData.goodsTable.GetTableData(GoodsTable.GoodsKey[GoodsEnum.Diamond]).Value -= price;

        //가챠갯수
        ServerData.userInfoTable.GetTableData(UiGachaPopup.Instance.GetGachaNumKey(itemType)).Value += amount;

        //무기
        for (int i = 0; i < amount; i++)
        {
            int randomIdx = Utils.GetRandomIdx(probs);
            var cellInfo = new GachaResultCellInfo();

            cellInfo.amount = 1;
            cellInfo.itemData = itemDatas[randomIdx];
            gachaResultCellInfos.Add(cellInfo);

            switch (itemType)
            {
                case ItemType.SubWeapon:
                {
                    var tableData = TableManager.Instance.subWeapon.dataArray[itemDatas[randomIdx].idx];
                    ServerData.subWeaponServerTable.UpData(tableData, cellInfo.amount);
                }
                    break;
                case ItemType.Charm:
                {
                    var tableData = TableManager.Instance.charm.dataArray[itemDatas[randomIdx].idx];
                    ServerData.charmServerTable.UpData(tableData, cellInfo.amount);
                }
                    break;
                case ItemType.Norigae:
                {
                    var tableData = TableManager.Instance.norigae.dataArray[itemDatas[randomIdx].idx];
                    ServerData.norigaeServerTable.UpData(tableData, cellInfo.amount);
                }
                    break;
                case ItemType.Skill:
                {
                    var tableData = TableManager.Instance.skillTable.dataArray[itemDatas[randomIdx].idx];
                    ServerData.skillServerTable.UpData(tableData, cellInfo.amount);
                }
                    break;
            }
 
            serverUpdateList.Add(itemDatas[randomIdx].idx);
        }

        SyncServer(serverUpdateList, price, serverUpdateList.Count);

       // DailyMissionManager.UpdateDailyMission(DailyMissionKey.GachaWeapon, amount);


        UiGachaResultView.Instance.Initialize(gachaResultCellInfos, () => { OnClickOpenButton(lastGachaIdx); });

       // SoundManager.Instance.PlaySound("Reward");

        //  UiTutorialManager.Instance.SetClear(TutorialStep._10_GetWeaponInShop);
    }

    private List<float> SetProbAndItemDatas(ItemType itemType,int gachaLevel)
    {
        probs.Clear();
        itemDatas.Clear();

        switch (itemType)
        {
            case ItemType.SubWeapon:
            {
                var tableData = TableManager.Instance.subWeapon.dataArray;
                
                for (int i = 0; i < tableData.Length; i++)
                {
                    itemDatas.Add(new ItemData(itemType,tableData[i].Id,tableData[i].Grade));
                    
                    if (gachaLevel == 0)
                    {
                        probs.Add(tableData[i].Gachalv1);
                    }
                    else if (gachaLevel == 1)
                    {
                        probs.Add(tableData[i].Gachalv2);
                    }
                    else if (gachaLevel == 2)
                    {
                        probs.Add(tableData[i].Gachalv3);
                    }
                    else if (gachaLevel == 3)
                    {
                        probs.Add(tableData[i].Gachalv4);
                    }
                    else if (gachaLevel == 4)
                    {
                        probs.Add(tableData[i].Gachalv5);
                    }
                    else if (gachaLevel == 5)
                    {
                        probs.Add(tableData[i].Gachalv6);
                    }
                    else if (gachaLevel == 6)
                    {
                        probs.Add(tableData[i].Gachalv7);
                    }
                    else if (gachaLevel == 7)
                    {
                        probs.Add(tableData[i].Gachalv8);
                    }
                    else if (gachaLevel == 8)
                    {
                        probs.Add(tableData[i].Gachalv9);
                    }
                    else if (gachaLevel == 9)
                    {
                        probs.Add(tableData[i].Gachalv10);
                    }
                }

            } break;
            case ItemType.Charm:
            {
                var tableData = TableManager.Instance.charm.dataArray;
                
                for (int i = 0; i < tableData.Length; i++)
                {
                    itemDatas.Add(new ItemData(itemType,tableData[i].Id,tableData[i].Grade));
                    
                    if (gachaLevel == 0)
                    {
                        probs.Add(tableData[i].Gachalv1);
                    }
                    else if (gachaLevel == 1)
                    {
                        probs.Add(tableData[i].Gachalv2);
                    }
                    else if (gachaLevel == 2)
                    {
                        probs.Add(tableData[i].Gachalv3);
                    }
                    else if (gachaLevel == 3)
                    {
                        probs.Add(tableData[i].Gachalv4);
                    }
                    else if (gachaLevel == 4)
                    {
                        probs.Add(tableData[i].Gachalv5);
                    }
                    else if (gachaLevel == 5)
                    {
                        probs.Add(tableData[i].Gachalv6);
                    }
                    else if (gachaLevel == 6)
                    {
                        probs.Add(tableData[i].Gachalv7);
                    }
                    else if (gachaLevel == 7)
                    {
                        probs.Add(tableData[i].Gachalv8);
                    }
                    else if (gachaLevel == 8)
                    {
                        probs.Add(tableData[i].Gachalv9);
                    }
                    else if (gachaLevel == 9)
                    {
                        probs.Add(tableData[i].Gachalv10);
                    }
                }

            } break;
            
            case ItemType.Norigae:
            {
                var tableData = TableManager.Instance.norigae.dataArray;
                
                for (int i = 0; i < tableData.Length; i++)
                {
                    itemDatas.Add(new ItemData(itemType,tableData[i].Id,tableData[i].Grade));
                    
                    if (gachaLevel == 0)
                    {
                        probs.Add(tableData[i].Gachalv1);
                    }
                    else if (gachaLevel == 1)
                    {
                        probs.Add(tableData[i].Gachalv2);
                    }
                    else if (gachaLevel == 2)
                    {
                        probs.Add(tableData[i].Gachalv3);
                    }
                    else if (gachaLevel == 3)
                    {
                        probs.Add(tableData[i].Gachalv4);
                    }
                    else if (gachaLevel == 4)
                    {
                        probs.Add(tableData[i].Gachalv5);
                    }
                    else if (gachaLevel == 5)
                    {
                        probs.Add(tableData[i].Gachalv6);
                    }
                    else if (gachaLevel == 6)
                    {
                        probs.Add(tableData[i].Gachalv7);
                    }
                    else if (gachaLevel == 7)
                    {
                        probs.Add(tableData[i].Gachalv8);
                    }
                    else if (gachaLevel == 8)
                    {
                        probs.Add(tableData[i].Gachalv9);
                    }
                    else if (gachaLevel == 9)
                    {
                        probs.Add(tableData[i].Gachalv10);
                    }
                }

            } break;
            case ItemType.Skill:
            {
                var tableData = TableManager.Instance.skillTable.dataArray;
                
                for (int i = 0; i < tableData.Length; i++)
                {
                    itemDatas.Add(new ItemData(itemType,tableData[i].Id,tableData[i].Grade));
                    
                    if (gachaLevel == 0)
                    {
                        probs.Add(tableData[i].Gachalv1);
                    }
                    else if (gachaLevel == 1)
                    {
                        probs.Add(tableData[i].Gachalv2);
                    }
                    else if (gachaLevel == 2)
                    {
                        probs.Add(tableData[i].Gachalv3);
                    }
                    else if (gachaLevel == 3)
                    {
                        probs.Add(tableData[i].Gachalv4);
                    }
                    else if (gachaLevel == 4)
                    {
                        probs.Add(tableData[i].Gachalv5);
                    }
                    else if (gachaLevel == 5)
                    {
                        probs.Add(tableData[i].Gachalv6);
                    }
                    else if (gachaLevel == 6)
                    {
                        probs.Add(tableData[i].Gachalv7);
                    }
                    else if (gachaLevel == 7)
                    {
                        probs.Add(tableData[i].Gachalv8);
                    }
                    else if (gachaLevel == 8)
                    {
                        probs.Add(tableData[i].Gachalv9);
                    }
                    else if (gachaLevel == 9)
                    {
                        probs.Add(tableData[i].Gachalv10);
                    }
                }

            } break;
        }

        return probs;
    }

    //서버 갱신만
    private void SyncServer(List<int> serverUpdateList, int price, int gachaCount)
    {
        List<TransactionValue> transactionList = new List<TransactionValue>();

        //재화
        Param goodsParam = new Param();
        goodsParam.Add(GoodsTable.GoodsKey[GoodsEnum.Diamond], ServerData.goodsTable.GetTableData(GoodsTable.GoodsKey[GoodsEnum.Diamond]).Value);

        //가챠횟수
        Param gachaNumParam = new Param();
        gachaNumParam.Add(UiGachaPopup.Instance.GetGachaNumKey(itemType), ServerData.userInfoTable.GetTableData(UiGachaPopup.Instance.GetGachaNumKey(itemType)).Value);

        //무기
        switch (itemType)
        {
            case ItemType.SubWeapon:
            {
                Param subWeaponParam = new Param();
        
                var table = TableManager.Instance.subWeapon.dataArray;
        
                var tableDatas = ServerData.subWeaponServerTable.TableDatas;
        
                for (int i = 0; i < table.Length; i++)
                {
                    if (serverUpdateList != null && serverUpdateList.Contains(table[i].Id) == false) continue;

                    string key = table[i].Stringid;
                    //hasitem 1
                    subWeaponParam.Add(key, tableDatas[key].ConvertToString());
                }
                
                //무기
                transactionList.Add(TransactionValue.SetUpdateV2(SubWeaponServerTable.tableName, SubWeaponServerTable.Indate,Backend.UserInDate,subWeaponParam));

            } 
                break;
            case ItemType.Charm:
            {
                Param charmParam = new Param();
        
                var table = TableManager.Instance.charm.dataArray;
        
                var tableDatas = ServerData.charmServerTable.TableDatas;
        
                for (int i = 0; i < table.Length; i++)
                {
                    if (serverUpdateList != null && serverUpdateList.Contains(table[i].Id) == false) continue;

                    string key = table[i].Stringid;
                    //hasitem 1
                    charmParam.Add(key, tableDatas[key].ConvertToString());
                }
                
                //무기
                transactionList.Add(TransactionValue.SetUpdateV2(CharmServerTable.tableName, CharmServerTable.Indate,Backend.UserInDate,charmParam));

            } 
                break;
            case ItemType.Norigae:
            {
                Param norigaeParam = new Param();
        
                var table = TableManager.Instance.norigae.dataArray;
        
                var tableDatas = ServerData.norigaeServerTable.TableDatas;
        
                for (int i = 0; i < table.Length; i++)
                {
                    if (serverUpdateList != null && serverUpdateList.Contains(table[i].Id) == false) continue;

                    string key = table[i].Stringid;
                    //hasitem 1
                    norigaeParam.Add(key, tableDatas[key].ConvertToString());
                }
                
                //무기
                transactionList.Add(TransactionValue.SetUpdateV2(NorigaeServerTable.tableName, NorigaeServerTable.Indate,Backend.UserInDate,norigaeParam));

            } 
                break;
            
            case ItemType.Skill:
            {
                Param skillParam = new Param();
        
                var table = TableManager.Instance.skillTable.dataArray;
        
                var tableDatas = ServerData.skillServerTable.TableDatas;
        
                for (int i = 0; i < table.Length; i++)
                {
                    if (serverUpdateList != null && serverUpdateList.Contains(table[i].Id) == false) continue;

                    string key = table[i].Stringid;
                    //hasitem 1
                    skillParam.Add(key, tableDatas[key].ConvertToString());
                }
                
                //무기
                transactionList.Add(TransactionValue.SetUpdateV2(SkillServerTable.tableName, SkillServerTable.Indate,Backend.UserInDate,skillParam));

            } 
                break;
        }
   
        //

        //재화
        transactionList.Add(TransactionValue.SetUpdateV2(GoodsTable.tableName, GoodsTable.Indate, Backend.UserInDate,goodsParam));
        //가챠횟수
        transactionList.Add(TransactionValue.SetUpdateV2(UserInfoTable.tableName, UserInfoTable.Indate,Backend.UserInDate, gachaNumParam));
    
        ServerData.SendTransaction(transactionList);
    }
}