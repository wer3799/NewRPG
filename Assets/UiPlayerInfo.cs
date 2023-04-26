using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;

public class UiPlayerInfo : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nickName;

    private void Start()
    {
        Subscribe();
    }

    private void Subscribe()
    {
        PlayerData.Instance.NickName.AsObservable().Subscribe(e => { nickName.SetText(e); }).AddTo(this);
    }
}