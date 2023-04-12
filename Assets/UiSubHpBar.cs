using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiSubHpBar : SingletonMono<UiSubHpBar>
{
    [SerializeField]
    private Image greenRenderer;

    [SerializeField]
    private Image greyRenderer;

    [SerializeField]
    private GameObject rootObject;

    private float fixedLerpSpeed = 5f;

    private Coroutine greyRoutine;

    private void Start()
    {
        rootObject.SetActive(false);
    }

    public void ShowGauge(bool show)
    {
        rootObject.SetActive(show);

        if (show)
        {
            ResetGauge();

            if (greyRoutine != null)
            {
                StopCoroutine(greyRoutine);
            }

            greyRoutine = StartCoroutine(GreyRoutine());
        }
        else
        {
            if (greyRoutine != null)
            {
                StopCoroutine(greyRoutine);
            }
        }
    }

    private void ResetGauge()
    {
        greenRenderer.fillAmount = 1f;
        greyRenderer.fillAmount = 1f;
    }

    private IEnumerator GreyRoutine()
    {
        while (true)
        {
            float lerpValue = Mathf.Lerp(greyRenderer.fillAmount, greenRenderer.fillAmount, Time.deltaTime * fixedLerpSpeed);

            greyRenderer.fillAmount = lerpValue;

            yield return null;
        }
    }

    public void UpdateGauge(double currentHp, double maxHp)
    {
        if (maxHp == 0f) return;

        greenRenderer.fillAmount = (float)(currentHp / maxHp);
    }
}