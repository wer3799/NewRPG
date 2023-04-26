using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class TimeOutContentsBase : MonoBehaviour
{
    public void Start()
    {
        
        Subscribe();
        
        StartTimer();
        
    }

    private void StartTimer()
    {
        var conTentsData = TableManager.Instance.MainContentsContainer[ContentsMakeController.Instance.currentContentsType.ToString()];
        
        UiContentsTimer.Instance.StartTimer(conTentsData.Timer);
    }

    private void Subscribe()
    {
        UiContentsTimer.Instance.whenTimerEnd.AsObservable().Subscribe(WhenTimerEnd).AddTo(this);
    }

    public virtual void WhenTimerEnd(Unit unit)
    {
        
    }
}