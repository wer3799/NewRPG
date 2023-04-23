using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Coffee.UIEffects;

public struct ItemData
{
    public ItemData(ItemType itemType, int idx, int grade)
    {
        this.itemType = itemType;

        this.idx = idx;

        this.grade = grade;
    }

    public ItemType itemType;

    public int idx;

    public int grade;
}

public class ItemView : MonoBehaviour
{
    [SerializeField]
    private Image bg;

    [SerializeField]
    private Image weaponIcon;

    [SerializeField]
    private Image skillIcon;

    [SerializeField]
    private TextMeshProUGUI gradeText;

    [SerializeField]
    private TextMeshProUGUI amountText;

    [SerializeField]
    private TextMeshProUGUI lvText;

    [SerializeField]
    private TextMeshProUGUI gradeNumText;

    [SerializeField]
    private GameObject gradeNumObject;

    private bool initialized = false;

    private CompositeDisposable disposable = new CompositeDisposable();

    [SerializeField]
    private UIShiny uishiny;

    private ItemData itemData;

    public void Initialize(ItemData itemData)
    {
        this.itemData = itemData;

        // weaponIcon.sprite = itemData.icon;

        //this.gradeText.SetText(CommonResourceContainer.GetGradeFrame(itemData.grade));

        // this.gradeText.color = (CommonUiContainer.Instance.itemGradeColor[grade]);

        //int gradeText = 4 - (id % 4);

        gradeNumText.SetText($"{gradeText}등급");


        //bg.sprite = CommonUiContainer.Instance.itemGradeFrame[grade];

        Subscribe();


        //uishiny.width = ((float)grade / 3f) * 0.6f;
        // uishiny.brightness = ((float)grade / 3f) * 0.8f;
    }


    private void Subscribe()
    {
        disposable.Clear();

        switch (itemData.itemType)
        {
            case ItemType.SubWeapon:
            {
                string stringId = TableManager.Instance.subWeapon.dataArray[itemData.idx].Stringid;

                ServerData.subWeaponServerTable.TableDatas[stringId].amount.AsObservable().Subscribe(WhenCountChanged).AddTo(disposable);

                ServerData.subWeaponServerTable.TableDatas[stringId].level.AsObservable().Subscribe(WhenLevelChanged).AddTo(disposable);
            }
                break;
        }
    }

    private void WhenLevelChanged(int level)
    {
        lvText.SetText($"Lv.{level}");
    }

    private void WhenCountChanged(int amount)
    {
        UpdateAmountText();
    }

    private void UpdateAmountText()
    {
        switch (itemData.itemType)
        {
            case ItemType.SubWeapon:
            {
                var tableData = TableManager.Instance.subWeapon.dataArray[itemData.idx];

                amountText.SetText($"({ServerData.subWeaponServerTable.GetCurrentWeaponCount(tableData.Stringid)}/{tableData.Requireupgrade})");
            }
                break;
        }
    }

    private void OnDestroy()
    {
        disposable.Dispose();
    }
}