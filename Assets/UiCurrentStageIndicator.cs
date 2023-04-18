using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;

public class UiCurrentStageIndicator : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI description;

    void Start()
    {
        Subscribe();
    }

    private void Subscribe()
    {
        NormalStageController.Instance.MapTableData.AsObservable().Subscribe(e =>
        {
            string format = GameString.GetString("CurrentStageFormat");
            
            if (format != null)
            {
                description.SetText(string.Format(format, e.Id + 1));
          
            }
            else
            {
                description.SetText($"{e.Chapter}-{e.Stage}");
            }
            
        }).AddTo(this);
    }
}