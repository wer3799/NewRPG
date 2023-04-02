using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class PlayerStatusController : SingletonMono<PlayerStatusController>
{
    private bool canHit = true;

    private WaitForSeconds hitDelay = new WaitForSeconds(1f);
    public ReactiveProperty<double> maxHp { get; private set; } = new ReactiveProperty<double>(GameBalance.initHp);
    public ReactiveProperty<double> hp { get; private set; } = new ReactiveProperty<double>();

    public ReactiveCommand whenPlayerDead = new ReactiveCommand();

    private float damTextYOffect = 1f;

    private void Start()
    {
        Initialize();
        Subscribe();
        StartCoroutine(RecoverRoutine());
    }

    public bool IsPlayerDead()
    {
        return hp.Value <= 0f;
    }

    private IEnumerator RecoverRoutine()
    {
        WaitForSeconds recoverDelay = new WaitForSeconds(5.0f);

        while (true)
        {
            yield return recoverDelay;

            if (IsPlayerDead()) continue;

            float hpRecoverPer = PlayerStats.GetHpRecover();

            if (IsHpFull() == false && hpRecoverPer != 0f)
            {
                UpdateHp(maxHp.Value * hpRecoverPer);
            }
        }
    }

    private void UpdateHpMax()
    {
        maxHp.Value = PlayerStats.GetMaxHp();
    }


    private void Subscribe()
    {
        // ServerData.statusTable.GetTableData(StatusTable.HpLevel_Gold).Subscribe(e =>
        // {
        //     UpdateHpMax();
        // }).AddTo(this);
        //
        // ServerData.statusTable.GetTableData(StatusTable.HpPer_StatPoint).Subscribe(e =>
        // {
        //     UpdateHpMax();
        // }).AddTo(this);

        //레벨업때
        // ServerData.statusTable.GetTableData(StatusTable.Level).Subscribe(e =>
        // {
        //     SetHpMpFull();
        // }).AddTo(this);
    }

    private IEnumerator LateUpdateHp()
    {
        yield return null;
        UpdateHpMax();
    }

    private void Initialize()
    {
        UpdateHpMax();
        SetHpFull();
    }

    private void SetHpFull()
    {
        hp.Value = maxHp.Value;
    }

    private bool IsHpFull()
    {
        return hp.Value == maxHp.Value;
    }

    public void SetHpToMax()
    {
        hp.Value = maxHp.Value;
    }

    public void UpdateHp(double value, float percentDamage = 0)
    {
        //데미지입음
        if (value < 0)
        {
            if (canHit == false || IsPlayerDead() ) return;

            float damDecreaseValue = PlayerStats.GetDamDecreaseValue();

            value -= value * damDecreaseValue;

            StartCoroutine(HitDelayRoutine());
        }
        //회복함
        else
        {
            if (IsPlayerDead()) return;
        }


#if UNITY_EDITOR
        // Debug.Log($"Player damaged {value}");
#endif
        if (percentDamage == 0)
        {
            SpawnDamText(value);
            hp.Value += value;
        }
        else
        {
            value = maxHp.Value * -percentDamage;

            SpawnDamText(value);
            hp.Value += value;
        }


        hp.Value = Mathf.Clamp((float)hp.Value, 0f, (float)maxHp.Value);

        CheckDead();
    }


    private void CheckDead()
    {
        if (hp.Value <= 0)
        {
            whenPlayerDead.Execute();
        }
    }

    private void SpawnDamText(double value)
    {
        Vector2 position = Vector2.up * damTextYOffect + UnityEngine.Random.insideUnitCircle;

        // if (value < 0)
        // {
        //     BattleObjectManagerAllTime.Instance.SpawnDamageText(value * -1f, this.transform.position, DamTextType.Red);
        // }
        // else
        // {
        //     BattleObjectManagerAllTime.Instance.SpawnDamageText(value, this.transform.position, DamTextType.Green);
        // }
    }
    private IEnumerator HitDelayRoutine()
    {
        canHit = false;
        yield return hitDelay;
        canHit = true;
    }
}
