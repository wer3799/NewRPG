using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class DamageTestManager : TimeOutContentsBase
{
    [SerializeField]
    private Enemy bossPrefab;
    
    private void Start()
    {
        base.Start();
    }
    
    public override void WhenTimerEnd(Unit unit)
    {
        //결과팝업?

        //나가기
        ContentsMakeController.Instance.ExitCurrentContents();
    }

}
