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

    private ReactiveProperty<double> damageAccum = new ReactiveProperty<double>(0d);

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
        damageAccum.AsObservable().Subscribe(e => { UpdateDamageDescription(); }).AddTo(this);
    }

    public void SetDefault()
    {
        damageAccum.Value = 0;
    }

    private void UpdateDamageDescription()
    {
        description.SetText($"{damageAccum}");
    }

    
    //damage -값으로 들어옴
    public void UpdateDescription(double damage)
    {
        if (rootObject.activeInHierarchy == false)
        {
            rootObject.gameObject.SetActive(true);
        }

        damageAccum.Value -= damage;
    }

    private void OnDisable()
    {
        rootObject.SetActive(false);
    }
}