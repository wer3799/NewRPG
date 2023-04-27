using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class UiContents : MonoBehaviour
{
   [SerializeField]
   private GameObject exitButton;

   [SerializeField]
   private GameObject timerObject;

   [SerializeField]
   private GameObject bossDamageIndicator;
   
   [SerializeField]
   private List<GameObject> normalStageObjects;
   
   private void Start()
   {
      Subscribe();
   }

   private bool ShowExitButton(ContentsName name)
   {
      return name == ContentsName.DamageTest ||
             name == ContentsName.Tower0;
   }
   
   private bool ShowContentsTimer(ContentsName name)
   {
      return name == ContentsName.DamageTest ||
             name == ContentsName.Tower0;
   }

   private void Subscribe()
   {
      ContentsMakeController.Instance.currentContentsType.AsObservable().Subscribe(e =>
      {
         normalStageObjects.ForEach(obj=>obj.SetActive(e == ContentsName.NormalField));
         
         exitButton.SetActive(ShowExitButton(e));
         
         timerObject.SetActive(ShowContentsTimer(e));
         
      }).AddTo(this);

   }

   public void OnClickContentsExitButton()
   {
      ContentsMakeController.Instance.ExitCurrentContents();
   }
}
