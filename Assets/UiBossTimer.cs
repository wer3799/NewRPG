using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using Unity.VisualScripting;

public class UiBossTimer : SingletonMono<UiBossTimer>
{
    [SerializeField]
    private Image fieldBossTimer;

    [SerializeField]
    private TextMeshProUGUI fieldBossRemainSec;

    [SerializeField]
    private GameObject rootObject;
    
    public ReactiveCommand whenFieldBossTimerEnd = new ReactiveCommand();

    [SerializeField]
    private Coroutine timerRoutine;

    private void Start()
    {
        rootObject.SetActive(false);
    }

    public void StartBossTimer(int second)
    {
        rootObject.SetActive(true);
        
        if (timerRoutine != null)
        {
            StopCoroutine(timerRoutine);
        }
        
        timerRoutine = StartCoroutine(FieldBossRoutine(second));
    }

    public void StopBossTimer()
    {
        rootObject.SetActive(false);
        
        if (timerRoutine != null)
        {
            StopCoroutine(timerRoutine);
        }
    }
    
    
    private IEnumerator FieldBossRoutine(int timer)
    {
        float remainSec = timer;

        fieldBossTimer.fillAmount = 1f;

        while (remainSec > 0)
        {
            yield return null;
            remainSec -= Time.deltaTime;
            fieldBossTimer.fillAmount = (remainSec / (float)timer);
            fieldBossRemainSec.SetText($"남은시간 {(int)remainSec}");
        }

        fieldBossTimer.fillAmount = 0f;

        whenFieldBossTimerEnd.Execute();
        
        rootObject.SetActive(false);
    }

}
