using BackEnd;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiDoubleClickButton : MonoBehaviour
{
    [Serializable]
    public class PointerDownEvent : UnityEvent { }
    
    [SerializeField]
    private PointerDownEvent OnEvent;
    
    private int clickCount = 0;

    private Coroutine clickDelayRoutine;

    private WaitForSeconds doubleClickDelay = new WaitForSeconds(2f);

    [SerializeField]
    private string doubleClickDescription;

    private void OnDisable()
    {
        ResetState();
    }

    private void ResetState()
    {
        clickCount = 0;
    }

    public void OnClickButton()
    {
        if (clickCount > 2)
        {
            return;
        }
        
        clickCount++;

        if (clickCount == 1)
        {
            PopupManager.Instance.ShowAlarmMessage(doubleClickDescription);
        }
   
        if (clickCount == 2)
        {
            OnEvent.Invoke();
        }
        
        if (clickDelayRoutine != null)
        {
            CoroutineExecuter.Instance.StopCoroutine(clickDelayRoutine);
        }

        clickDelayRoutine = CoroutineExecuter.Instance.StartCoroutine(ClickDelayRoutine());
    }

    public IEnumerator ClickDelayRoutine()
    {
        
        yield return doubleClickDelay;
        
        clickCount = 0;
    }
}
