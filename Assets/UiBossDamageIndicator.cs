using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;

public class UiBossDamageIndicator : SingletonMono<UiBossDamageIndicator>
{
    [SerializeField]
    private TextMeshProUGUI description;

    [SerializeField]
    private GameObject rootObject;

    private ReactiveProperty<double> currentDamage = new ReactiveProperty<double>(0d);

    private void Start()
    {
        Subscribe();
    }

    public void HideIndiactor()
    {
        rootObject.SetActive(false);
    }

    private void Subscribe()
    {
        currentDamage.AsObservable().Subscribe(e => { UpdateDamageDescription(); }).AddTo(this);
    }

    public void SetDefault()
    {
        currentDamage.Value = 0;
    }

    private void UpdateDamageDescription()
    {
        description.SetText($"{currentDamage}");
    }

    
    //damage -값으로 들어옴
    public void UpdateDescription(double damage)
    {
        if (rootObject.activeInHierarchy == false)
        {
            rootObject.gameObject.SetActive(true);
        }

        currentDamage.Value = damage;
    }

    private void OnDisable()
    {
        rootObject.SetActive(false);
    }
}