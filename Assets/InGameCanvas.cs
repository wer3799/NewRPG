using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InGameCanvas : SingletonMono<InGameCanvas>
{
   [SerializeField]
   private GameObject rootObject;

   public void ShowInGameCanvas(bool show)
   {
      rootObject.SetActive(show);
   }
}
