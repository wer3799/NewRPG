using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static UiGachaResultView;

public class GachaResultViewCell : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI amountText;

    [SerializeField]
    private ItemView itemView;

    //[SerializeField]
    //private GameObject rareEffect;
    [SerializeField]
    private GameObject uniqueEffect;

    private static string GetUniqueKey = "GetUnique";

    [SerializeField]
    private Image openMask;

    public void Initialzie(ItemData itemData, int amount)
    {
        itemView.Initialize(itemData);
        amountText.SetText($"{amount}개");

        if (itemData.grade == 3)
        {
            //SoundManager.Instance.PlaySound(GetUniqueKey);
            PopupManager.Instance.ShowWhiteEffect();
        }

       // openMask.color = CommonUiContainer.Instance.itemGradeColor[itemData.grade];
      
    }
}
