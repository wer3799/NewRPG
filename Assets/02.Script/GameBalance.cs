using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBalance
{
    public readonly static float moveSpeed = 10f;
    public readonly static float jumpPower = 17f;
    public readonly static float doubleJumpPower = 25f;
}

public class GameBalance 
{
    public readonly static int sleepRewardMinValue = 600;
    //10시간
    public readonly static int sleepRewardMaxValue = 86400;
    public readonly static float sleepRewardRatio = 1f;
    public readonly static float StartingMoney = 0f;
    public readonly static int skillSlotGroupNum = 3;
    public readonly static int SkillAwakePlusNum = 10;
    
    public readonly static float initHp = 10;
    public readonly static float MaxDamTextNum = 100;
    public readonly static int effectActiveDistance = 15;
}

public static class DamageBalance
{
    public readonly static float baseMinDamage = 1f;
    public readonly static float baseMaxDamage = 1f;
    public static float GetRandomDamageRange()
    {
        return Random.Range(baseMinDamage , baseMaxDamage);
    }
}