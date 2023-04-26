using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiBossDamageIndicator : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI description;

    [SerializeField]
    private GameObject rootObject;

    public void UpdateDescription(double damage)
    {
        if (rootObject.activeInHierarchy == false)
        {
            rootObject.gameObject.SetActive(true);
        }
        description.SetText(Utils.ConvertBigNum(damage));
    }
    
    private void OnDisable()
    {
        rootObject.SetActive(false);
    }
}
