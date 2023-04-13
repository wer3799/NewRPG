using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using CodeStage.AntiCheat.ObscuredTypes;

public enum SkillCastType
{
    Player,
}

public class PlayerSkillCaster : SingletonMono<PlayerSkillCaster>
{
    [SerializeField]
    private PlayerMoveController playerMoveController;
    public PlayerMoveController PlayerMoveController => playerMoveController;

    public Dictionary<int, SkillBase> UserSkills { get; private set; } = new Dictionary<int, SkillBase>();

    public bool isSkillMoveRestriction = false;

    private string newWeaponKey1 = "weapon23";
    private string newWeaponKey2 = "weapon24";
    private float addRange = 0f;
    public bool UseSkill(int skillIdx)
    {
        bool canUserSkill = UserSkills[skillIdx].CanUseSkill();

        if (canUserSkill)
        {
            UserSkills[skillIdx].UseSkill();
        }

        return canUserSkill;
    }

    private void Start()
    {
        InitSkill();
    }
    // public void SetMoveRestriction(float time)
    // {
    //     if (time == 0f) return;
    //
    //     StartCoroutine(MoveRestrictionRoutine(time));
    // }

    // private IEnumerator MoveRestrictionRoutine(float time)
    // {
    //     isSkillMoveRestriction = true;
    //     yield return new WaitForSeconds(time);
    //     isSkillMoveRestriction = false;
    // }

    private void InitSkill()
    {
        for (int i = 0; i < TableManager.Instance.skillTable.dataArray.Length; i++)
        {
            var SkillTableData = TableManager.Instance.skillTable.dataArray[i];

            if (ServerData.skillServerTable.HasSkill(SkillTableData.Id))
            {
                Type elementType = Type.GetType(SkillTableData.Skillclassname);

                object classType = Activator.CreateInstance(elementType);

                var skillBase = classType as SkillBase;

                skillBase.Initialize(this.transform, SkillTableData, this);

                UserSkills.Add(SkillTableData.Id, skillBase);
            }
        }
    }

    public Collider2D[] GetEnemiesInCircle(Vector2 origin, float radius)
    {
        return Physics2D.OverlapCircleAll(origin, radius + addRange, LayerMasks.EnemyLayerMask);
    }

    public RaycastHit2D[] GetEnemiesInRaycast(Vector2 origin, Vector2 rayDirection, float length)
    {
        return Physics2D.RaycastAll(origin, rayDirection, length, LayerMasks.EnemyLayerMask);
    }

    public RaycastHit2D[] GetEnemiesInBoxcast(Vector2 origin, Vector2 rayDirection, float length, float size)
    {
        return Physics2D.BoxCastAll(origin, Vector2.one * (size + addRange), 0f, rayDirection, length, LayerMasks.EnemyLayerMask);
    }

    private string wallString = "Wall";
    public Vector2 GetRayHitWallPoint(Vector2 origin, Vector2 rayDirection, float length)
    {
        int hitLayer = LayerMasks.PlatformLayerMask_Ray + LayerMasks.EnemyWallLayerMask_Ray;

        var rayHits = Physics2D.RaycastAll(origin, rayDirection, length, hitLayer);

        for (int i = 0; i < rayHits.Length; i++)
        {
            if (rayHits[i].collider.gameObject.tag.Equals(wallString))
            {
                return rayHits[i].point - rayDirection.normalized * 0.5f;
            }
        }
        return Vector2.zero;
    }

    public Vector2 GetRayHitPlatformPoint(Vector2 origin, Vector2 rayDirection, float length, bool ignoreEnemyWall = false)
    {
        int hitLayer = 0;

        if (ignoreEnemyWall == false)
        {
            hitLayer = LayerMasks.PlatformLayerMask_Ray + LayerMasks.EnemyWallLayerMask_Ray;
        }
        else
        {
            hitLayer = LayerMasks.PlatformLayerMask_Ray;
        }

        var rayHits = Physics2D.RaycastAll(origin, rayDirection, length, hitLayer);

        for (int i = 0; i < rayHits.Length; i++)
        {
            return rayHits[i].point;
        }

        return Vector2.zero;
    }


    public void PlayAttackAnim()
    {
        PlayerViewController.Instance.SetCurrentAnimation(PlayerViewController.AnimState.attack);
    }

    public Vector3 GetSkillCastingPosOffset(SkillTableData tableData)
    {
        return tableData.Activeoffset * Vector2.right * (playerMoveController.MoveDirection == MoveDirection.Right ? 1 : -1);
    }
    
    private Dictionary<int, EnemyHpController> agentHpControllers = new Dictionary<int, EnemyHpController>();
    
    public IEnumerator ApplyDamage(Collider2D hitEnemie, SkillTableData skillInfo, double damage, bool playSound)
    {
        EnemyHpController EnemyHpController;
        
        int instanceId = hitEnemie.GetInstanceID();
        
        if (agentHpControllers.ContainsKey(instanceId) == false)
        {
            agentHpControllers.Add(instanceId, hitEnemie.gameObject.GetComponent<EnemyHpController>());

            EnemyHpController = agentHpControllers[instanceId];
        }
        else
        {
            EnemyHpController = agentHpControllers[instanceId];
        }
        
        double totalDamage = damage;
        
        //데미지는 한프레임에 적용
        if (EnemyHpController.gameObject == null || EnemyHpController.gameObject.activeInHierarchy == false)
        {
            yield break;
        }
        else
        {
            EnemyHpController.UpdateHp(-totalDamage);
        }
        
        bool spawnDamText = SettingData.ShowDamageFont.Value == 1;
        
        // if (spawnDamText)
        // {
            EnemyHpController.SpawnDamText(false, false, totalDamage);
        // }
        
        // //이펙트는 최대 10개까지만 출력
        // for (int hit = 0; hit < hitCount && hit < 10; hit++)
        // {
        //     if (spawnDamText)
        //     {
        //         EnemyHpController.SpawnDamText(isCritical, isSuperCritical, calculatedDam);
        //     }
        //
        //     //사운드
        //     //시전할때 사운드 있어서 따로재생X
        //     if (hit != 0 && playSound)
        //     {
        //         SoundManager.Instance.PlaySound(skillInfo.Soundname);
        //     }
        //
        //     //이펙트
        //     if (string.IsNullOrEmpty(skillInfo.Hiteffectname) == false &&
        //         Vector3.Distance(this.transform.position, hitEnemie.gameObject.transform.position) < GameBalance.effectActiveDistance)
        //     {
        //         Vector3 spawnPos = hitEnemie.gameObject.transform.position + Vector3.forward * -1f + Vector3.up * 0.3f;
        //         spawnPos += (Vector3)UnityEngine.Random.insideUnitCircle * 0.5f;
        //         spawnPos += (Vector3)Vector3.back;
        //         EffectManager.SpawnEffectAllTime(skillInfo.Hiteffectname, spawnPos, limitSpawnSize: true);
        //     }
        //
        //     float tick = 0f;
        //
        //     while (tick < 0.05f)
        //     {
        //         tick += Time.deltaTime;
        //         yield return null;
        //     }
        // }
    }

    private new void OnDestroy()
    {
        base.OnDestroy();
        StopAllCoroutines();
    }


}
