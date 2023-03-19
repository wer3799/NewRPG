using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBalance
{
    public readonly static float moveSpeed = 8f;
    public readonly static float jumpPower = 17f;
    public readonly static float doubleJumpPower = 25f;
}

public class GameBalance 
{
    public readonly static int sleepRewardMinValue = 600;
    //10시간
    public readonly static int sleepRewardMaxValue = 86400;
    public readonly static float sleepRewardRatio = 1f;
}
