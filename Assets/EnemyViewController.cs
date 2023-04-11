using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class EnemyViewController : MonoBehaviour
{
    [SerializeField]
    private SkeletonAnimation skeletonRenderer;

    [SerializeField]
    private float fadeSpeed = 0.1f;


    private void OnEnable()
    {
        StartCoroutine(fadeRoutine());
    }

    IEnumerator fadeRoutine()
    {
        yield return null;
        
        float alpha = 0f;

        while (alpha < 1f)
        {
            skeletonRenderer.skeleton.A = alpha;
            alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        skeletonRenderer.skeleton.A = 1f;
    }
}