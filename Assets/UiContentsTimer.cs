using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;

public class UiContentsTimer : SingletonMono<UiContentsTimer>
{
    [SerializeField]
    private TextMeshProUGUI timeDescription;

    private float maxSecond;

    public ReactiveCommand whenTimerEnd = new ReactiveCommand();

    public void StartTimer(int maxSecond)
    {
        this.maxSecond = maxSecond;

        if (timerRoutine != null)
        {
            StopCoroutine(timerRoutine);
        }

        timerRoutine = StartCoroutine(TimerRoutine());
    }

    private Coroutine timerRoutine;

    private IEnumerator TimerRoutine()
    {
        float tick = maxSecond;
        
        int tickSecond = (int)tick;
        
        SetTimerText(tickSecond);

        while (tick >= 0f)
        {
            tick -= Time.deltaTime;
            
            if (tickSecond != (int)tick)
            {
                SetTimerText(tickSecond);
            }
                
            tickSecond = (int)tick;

            yield return null;
        }

        SetTimerText(0);
        
        whenTimerEnd.Execute();
    }

    private void SetTimerText(int second)
    {
        timeDescription.SetText($"{second}초");
    }
}