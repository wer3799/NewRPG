using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UniRx;

public class UiExpGauge : SingletonMono<UiExpGauge>
{
    [SerializeField]
    private Image gauge;

    [SerializeField]
    private TextMeshProUGUI gaugeText;

    [SerializeField]
    private TextMeshProUGUI levelDescription;
    
    public bool updateGaugeFlag = true;

    private void LateUpdate()
    {
        if (updateGaugeFlag)
        {
            UpdateUi();
            updateGaugeFlag = false;
        }
    }

    public void UpdateUi()
    {
        int currentLevel = (int)ServerData.growthTable.GetTableData(GrowthTable.Level).Value;
        
        double currentExp = ServerData.growthTable.GetTableData(GrowthTable.Exp).Value;
        
        double maxExp = GrowthManager.Instance.maxExp.Value;
        
        levelDescription.SetText($"LV : {currentLevel}");

        gauge.fillAmount = (float)(currentExp / maxExp);

        gaugeText.SetText($"{Utils.ConvertBigNum(currentExp)}/{Utils.ConvertBigNum(maxExp)}({(int)(currentExp / maxExp * 100f)}%)");
    }
}