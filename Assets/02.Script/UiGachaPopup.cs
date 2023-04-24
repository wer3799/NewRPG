using CodeStage.AntiCheat.ObscuredTypes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public enum ItemType
{
    SubWeapon,
    Charm,
    Norigae,
    Skill,
    Max
}

public class UiGachaPopup : SingletonMono<UiGachaPopup>
{
    //누적
    public GachaLevelData gachaLevelData;

    [SerializeField]
    private List<Image> gaugeImage;

    [SerializeField]
    private List<TextMeshProUGUI> gaugeDescription;

    [SerializeField]
    private List<TextMeshProUGUI> gachaLevelText;

    public static string GachaNumKey = "gachaNum";

    private void Start()
    {
        Subscribe();
    }

    private void Subscribe()
    {
        for (int i = 0; i < (int)ItemType.Max; i++)
        {
            int idx = i;

            ServerData.userInfoTable.GetTableData($"{GachaNumKey}{idx}").AsObservable().Subscribe(e => { WhenGachaNumChanged((ItemType)idx, e); }).AddTo(this);
        }
    }

    public List<int> GetGachaLevelMinNum(ItemType itemType)
    {
        return gachaLevelData.GachaLevelMinNum[(int)itemType];
    }

    private void WhenGachaNumChanged(ItemType itemType, double num)
    {
        int gachaLevel = GachaLevel(itemType);

        int gachaIdx = (int)itemType;

        gachaLevelText[gachaIdx].text = $"LV : {gachaLevel + 1}";

        int current = (int)num;

        List<int> gachaLevelMinNum = GetGachaLevelMinNum(itemType);

        //만렙아닐때
        if (gachaLevel < gachaLevelMinNum.Count - 1)
        {
            int prefMaxCount = gachaLevelMinNum[gachaLevel];
            int nextMaxCount = gachaLevelMinNum[gachaLevel + 1];

            gaugeDescription[gachaIdx].text = $"{current - prefMaxCount}/{nextMaxCount - prefMaxCount}";

            gaugeImage[gachaIdx].fillAmount = (float)(current - prefMaxCount) / (float)(nextMaxCount - prefMaxCount);
        }
        //만렙일때
        else
        {
            gaugeDescription[gachaIdx].text = $"LV : {gachaLevel + 1}(MAX)";

            gachaLevelText[gachaIdx].text = $"MAX";

            gaugeImage[gachaIdx].fillAmount = 1f;
        }
    }


    public string GetGachaNumKey(ItemType type)
    {
        return $"{GachaNumKey}{(int)type}";
    }

    public int GachaLevel(ItemType type)
    {
        int gachaNum = (int)ServerData.userInfoTable.GetTableData($"{GachaNumKey}{(int)type}").Value;

        int gachaLevel = 0;

        List<int> gacbaLevelInfo = GetGachaLevelMinNum(type);
        ;

        for (int i = 0; i < gacbaLevelInfo.Count; i++)
        {
            if (gachaNum >= gacbaLevelInfo[i])
            {
                gachaLevel = i;
            }
            else
            {
                break;
            }
        }

        return gachaLevel;
    }
}