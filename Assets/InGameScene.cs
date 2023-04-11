using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameScene : MonoBehaviour
{
     private void Awake()
     {
         InGameCanvas.Instance.ShowInGameCanvas(true);
     }
}
