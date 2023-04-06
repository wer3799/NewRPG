using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class PlayerViewController : SingletonMono<PlayerViewController>
{
    [SerializeField]
    private SkeletonAnimation skeletonGraphic;

    private AnimState animState;

    private string CurrentAnimation;

    private const string Anim_Idle = "idle";
    private const string Anim_Run = "walk";
    private const string Anim_Attack = "attack1";
    private const string Anim_Attack2 = "attack2";
    private const string Anim_Attack3 = "attack3";
    private const string Attack = "attack";

    private Coroutine attackAnimEndRoutine;

    private WaitForSeconds attackAnimDelay = new WaitForSeconds(0.2f);

    private IEnumerator AttackAnimEndRoutine()
    {
        yield return attackAnimDelay;
        CurrentAnimation = string.Empty;
        attackAnimEndRoutine = null;
    }

    public enum AnimState
    {
        idle, run, attack
    }

    private void SetAnimation(string animName)
    {
        return;
        
        if (attackAnimEndRoutine != null || (CurrentAnimation == animName && animName.Contains(Attack) == false)) return;

        bool isAttackAnim = animName.Contains(Attack);

        if (isAttackAnim)
        {
            skeletonGraphic.timeScale = 4f;
            if (attackAnimEndRoutine != null)
            {
                StopCoroutine(attackAnimEndRoutine);
            }

            attackAnimEndRoutine = StartCoroutine(AttackAnimEndRoutine());
        }
        else
        {
            skeletonGraphic.timeScale = 2f;
        }

        bool loop = animName.Equals(Anim_Idle) || animName.Equals(Anim_Run);


        skeletonGraphic.AnimationState.SetAnimation(0, animName, loop);

        CurrentAnimation = animName;
    }
    int attackIdx = 0;
    public void SetCurrentAnimation(AnimState state)
    {
        //attack idle run
        switch (state)
        {
            case AnimState.attack:
                {
                    if (attackIdx == 0)
                    {
                        SetAnimation(Anim_Attack);
                        attackIdx++;
                    }
                    else if (attackIdx == 1)
                    {
                        SetAnimation(Anim_Attack);
                        attackIdx++;
                    }
                    else
                    {
                        SetAnimation(Anim_Attack);
                        attackIdx = 0;
                    }

                }
                break;
            case AnimState.idle:
                {
                    SetAnimation(Anim_Idle);
                }
                break;
            case AnimState.run:
                {
                    SetAnimation(Anim_Run);
                }
                break;
        }
    }
}
