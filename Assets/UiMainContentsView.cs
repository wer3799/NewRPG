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

        ContentsType type = Enum.Parse<ContentsType>(this.mainContentsData.Contentstype);

        contentsName.SetText(type.ToString());
    }

    public void OnClickEnterButton()
    {
        UiMainContentsBoard.Instance.LoadContents(Enum.Parse<ContentsType>(this.mainContentsData.Contentstype));
    }
}