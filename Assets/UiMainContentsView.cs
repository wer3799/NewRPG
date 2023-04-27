using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;

public class UiMainContentsView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI contentsName;

    [SerializeField]
    private TextMeshProUGUI contentsScore;

    private MainContentsData mainContentsData;

    public void Initialize(MainContentsData mainContentsData)
    {
        this.mainContentsData = mainContentsData;

        ContentsName name = Enum.Parse<ContentsName>(this.mainContentsData.Contentsname);

        contentsName.SetText(name.ToString());

        Subscribe();
    }

    private void Subscribe()
    {
        ServerData.clearInfoServerTable.TableDatas[mainContentsData.Contentsname].AsObservable().Subscribe(e => { contentsScore.SetText($"{Utils.ConvertBigNum(e)}점"); }).AddTo(this);
    }

    public void OnClickEnterButton()
    {
        UiMainContentsBoard.Instance.LoadContents(Enum.Parse<ContentsName>(this.mainContentsData.Contentsname));
    }
}