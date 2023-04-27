using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiMainContentsView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI contentsName;

    private MainContentsData mainContentsData;

    public void Initialize(MainContentsData mainContentsData)
    {
        this.mainContentsData = mainContentsData;

        ContentsName name = Enum.Parse<ContentsName>(this.mainContentsData.Contentsname);

        contentsName.SetText(name.ToString());
    }

    public void OnClickEnterButton()
    {
        UiMainContentsBoard.Instance.LoadContents(Enum.Parse<ContentsName>(this.mainContentsData.Contentsname));
    }
}