using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentsMakeController : SingletonMono<ContentsMakeController>
{
    public ContentsType currentContentsType = ContentsType.NormalField;

    public void StartContents(ContentsType type)
    {
        if (currentContentsType == ContentsType.NormalField)
        {
            //TODO : 보스 끄고 컨텐츠로 넘어가게
            if (NormalStageController.Instance.IsBossState == true)
            {
                PopupManager.Instance.ShowAlarmMessage("스테이지 보스가 있으면 컨텐츠를 시작하실 수 없습니다.");
                return;
            }
        }
        
        //스테이지 비활성화
        NormalStageController.Instance.DisableStage();
        
        PlayerMoveController.Instance.SetPlayerToOriginPos();

        PopupManager.Instance.ShowStageChangeEffect();
        
        //프리팹 생성
        currentContentsType = type;

        Debug.LogError($"{type} Loaded");
    }

    public void ExitCurrentContents()
    {
        currentContentsType = ContentsType.NormalField;
     
        PopupManager.Instance.ShowStageChangeEffect();
        
        PlayerMoveController.Instance.SetPlayerToOriginPos();

        NormalStageController.Instance.MakeStage();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartContents(ContentsType.Test0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ExitCurrentContents();
        }
    }
#endif
}