using System.Collections;
using System.Collections.Generic;
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
