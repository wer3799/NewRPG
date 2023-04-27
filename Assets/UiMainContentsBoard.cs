using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiMainContentsBoard : SingletonMono<UiMainContentsBoard>
{
    [SerializeField]
    private UiMainContentsView uiMainContentsView;

    [SerializeField]
    private Transform cellParents;

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        var tableData = TableManager.Instance.mainContents.dataArray;

        for (int i = 0; i < tableData.Length; i++)
        {
            if (tableData[i].CONTENTSWHERE != ContentsWhere.MainContentsMenu) continue;

            var cell = Instantiate<UiMainContentsView>(uiMainContentsView, cellParents);

            cell.Initialize(tableData[i]);
        }
    }

    public void LoadContents(ContentsName contentsName)
    {
        bool loadSuccess = ContentsMakeController.Instance.StartContents(contentsName);

        if (loadSuccess)
        {
            ClosePopup();
        }
    }

    public void ClosePopup()
    {
        this.gameObject.SetActive(false);
    }
}