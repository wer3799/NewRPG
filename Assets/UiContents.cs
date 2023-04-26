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
   
   private void Start()
   {
      Subscribe();
   }

   private bool ShowExitButton(ContentsType type)
   {
      return type == ContentsType.DamageTest ||
             type == ContentsType.Tower0;
   }
   
   private bool ShowContentsTimer(ContentsType type)
   {
      return type == ContentsType.DamageTest ||
             type == ContentsType.Tower0;
   }

   private void Subscribe()
   {
      ContentsMakeController.Instance.currentContentsType.AsObservable().Subscribe(e =>
      {
         
         exitButton.SetActive(ShowExitButton(e));
         
         timerObject.SetActive(ShowContentsTimer(e));
         
      }).AddTo(this);

   }

   public void OnClickContentsExitButton()
   {
      ContentsMakeController.Instance.ExitCurrentContents();
   }
}
