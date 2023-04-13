using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBase
{
    protected Transform playerTr;
    public SkillTableData skillInfo { get; private set; }
    protected PlayerSkillCaster playerSkillCaster;
    protected WaitForSeconds damageApplyInterval = new WaitForSeconds(0.05f);

    public void Initialize(Transform playerTr, SkillTableData skillInfo, PlayerSkillCaster playerSkillCaster)
    {
        this.playerTr = playerTr;
        this.skillInfo = skillInfo;
        this.playerSkillCaster = playerSkillCaster;
    }

    public bool CanUseSkill()
    {
        //mp계산 뒤에서해야됨.실제 엠피 차감해서
        return !SkillCoolTimeManager.HasSkillCooltime(skillInfo.Id) && PlayerStatusController.Instance.IsPlayerDead() == false;
    }

    protected virtual double GetSkillDamage(SkillTableData skillInfo)
    {
        double apDamage = PlayerStats.GetCalculatedAttackPower();

        double skillDamagePer = ServerData.skillServerTable.GetSkillDamagePer(skillInfo.Id);

        return apDamage * skillDamagePer;
    }

    public virtual void UseSkill()
    {
        playerSkillCaster.PlayAttackAnim();

        SkillCoolTimeManager.SetActiveSkillCool(skillInfo.Id, SkillCoolTimeManager.GetSkillCoolTimeMax(skillInfo));

        SpawnActiveEffect();

        PlaySoundEfx(skillInfo.Soundname);
    }

    private void PlaySoundEfx(string soundKey)
    {
        //SoundManager.Instance.PlaySound(soundKey);
    }


    private void SpawnActiveEffect()
    {
        Transform targetTr = null;

        if (skillInfo.SKILLCASTTYPE == SkillCastType.Player)
        {
            targetTr = PlayerMoveController.Instance.transform;
        }

        MoveDirection moveDirection = PlayerMoveController.Instance.MoveDirection;

        Vector3 activeEffectSpawnPos = targetTr.position;

        Transform parent = skillInfo.Iseffectrootplayer ? targetTr : null;

        var effect = EffectManager.SpawnEffectAllTime(skillInfo.Activeeffectname1, activeEffectSpawnPos, parent);

        if (effect != null)
        {
            if (skillInfo.Iseffectrootplayer == false)
            {
                effect.transform.position = targetTr.position;

                effect.transform.localScale = new Vector3(1f, 1f, 1f);
            }
            else
            {
                effect.transform.localScale = new Vector3(Mathf.Abs(effect.transform.localScale.x) * (moveDirection == MoveDirection.Right ? 1f : -1f), effect.transform.localScale.y, effect.transform.localScale.z);
            }
        }
    }
}