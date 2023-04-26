using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class ContentsMakeController : SingletonMono<ContentsMakeController>
{
    public ReactiveProperty<ContentsType> currentContentsType = new ReactiveProperty<ContentsType>(ContentsType.NormalField);
    
    private GameObject contentsObject;
    
    
    public bool StartContents(ContentsType type)
    {
        if (currentContentsType.Value == type)
        {
            PopupManager.Instance.ShowAlarmMessage("컨텐츠 로드 불가");            
            return false;
        }
        
        if (currentContentsType.Value == ContentsType.NormalField)
        {
            //TODO : 보스 끄고 컨텐츠로 넘어가게
            if (NormalStageController.Instance.IsBossState == true)
            {
                PopupManager.Instance.ShowAlarmMessage("스테이지 보스가 있으면 컨텐츠를 시작하실 수 없습니다.");
                return false;
            }
        }
        
        //스테이지 비활성화
        NormalStageController.Instance.DisableStage();
        
        PlayerMoveController.Instance.SetPlayerToOriginPos();

        PopupManager.Instance.ShowStageChangeEffect();
        
        //프리팹 생성
        currentContentsType.Value = type;

        SpawnContentsObject(currentContentsType.Value);

        Debug.LogError($"{type} Loaded");

        return true;
    }

    private void SpawnContentsObject(ContentsType contentsType)
    {
        var prefab = Resources.Load<GameObject>($"Contents/{contentsType.ToString()}" );
        
        contentsObject = Instantiate(prefab);
    }

    public void ExitCurrentContents()
    {
        if (currentContentsType.Value == ContentsType.NormalField) return;
        
        currentContentsType.Value = ContentsType.NormalField;
     
        PopupManager.Instance.ShowStageChangeEffect();
        
        PlayerMoveController.Instance.SetPlayerToOriginPos();

        NormalStageController.Instance.MakeStage();
        
        Destroy(contentsObject);
        
        contentsObject = null;
    }

}