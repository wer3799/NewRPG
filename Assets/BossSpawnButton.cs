using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using BackEnd;
using I2.Loc;
using TMPro;

public class BossSpawnButton : MonoBehaviour
{
    [SerializeField]
    private Localize buttonDescription;

    private void Start()
    {
        Subscribe();
    }

    private void Subscribe()
    {
        ServerData.userInfoTable.GetTableData(UserInfoTable.CurrentStage).AsObservable().Subscribe(e =>
        {
            int nextStageId = (int)e + 1;

            int currentStage = (int)e;

            if (currentStage == TableManager.Instance.GetLastStageIdx())
            {
                //최고 단계
                buttonDescription.SetTerm("TopClearText");
                return;
            }

            if (nextStageId > currentStage + 1)
            {
                //보스 도전
                buttonDescription.SetTerm("StartStageBoss");
                return;
            }

            //다음 스테이지
            buttonDescription.SetTerm("NextStage");
        }).AddTo(this);
    }

    public void OnClickSpawnButton()
    {
        int currentStage = (int)ServerData.userInfoTable.GetTableData(UserInfoTable.CurrentStage).Value;

        if (currentStage == TableManager.Instance.GetLastStageIdx())
        {
            //최고 단계 입니다. 다음 업데이트를 기다려주세요!
            PopupManager.Instance.ShowAlarmMessage(GameString.GetString("TopClearDescription"));

            //UiAutoBoss.Instance.WhenToggleChanged(false);

            return;
        }

        int nextStageId = NormalStageController.Instance.MapTableData.Value.Id + 1;

        if (currentStage < nextStageId)
        {
            if (GameManager.contentsType != ContentsType.NormalField)
            {
                //필드보스를 소환할 수 없는 곳 입니다.
                PopupManager.Instance.ShowAlarmMessage(GameString.GetString("CantSummonFieldBoss"));
                //UiAutoBoss.Instance.WhenToggleChanged(false);
                return;
            }

            if (NormalStageController.Instance.IsBossState)
            {
                //이미 필드에 보스가 있습니다!
                PopupManager.Instance.ShowAlarmMessage(GameString.GetString("AlreadySpawnedFieldBoss"));
                //UiAutoBoss.Instance.WhenToggleChanged(false);
                return;
            }

            // if (UiAutoBoss.AutoMode.Value == false)
            // {
            //     //확인팝업
            //     PopupManager.Instance.ShowYesNoPopup(CommonString.Notice, "스테이지 보스를 소환합니까?\n\n<color=red>(1층에 소환됩니다.)</color>", () =>
            //     {
            //         PlayerSkillCaster.Instance.InitializeVisionSkill();
            //         MapInfo.Instance.SpawnBossEnemy();
            //     }, null);
            // }
            // else
            // {
            // }
            
            //확인팝업
            //스테이지 보스를 소환합니까?\n\n<color=red>(1층에 소환됩니다.)</color>
            PopupManager.Instance.ShowYesNoPopup(GameString.Notice, GameString.GetString("AskSpawnFieldBoss"), () =>
            {
                NormalStageController.Instance.StartStageBoss();
                // PlayerSkillCaster.Instance.InitializeVisionSkill();
                // MapInfo.Instance.SpawnBossEnemy();
            }, null);
            
  
        }
        else
        {
            PopupManager.Instance.ShowYesNoPopup(GameString.Notice, "다음 스테이지로 이동합니까?", () =>
            {
                NormalStageController.Instance.MoveNextStage();
            }, null);
        }
   
    }
}