using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.VFX;

public class ParticleEffect : PoolItem
{
    [SerializeField]
    private float disableTime = 1f;

    private static string Func_DisableObject = "DisableObject";

    [SerializeField]
    private ParticleSystem[] particles;

    [SerializeField]
    private VisualEffect[] visualEffects;
    
    private void OnValidate()
    {
        particles = GetComponentsInChildren<ParticleSystem>(true);
        visualEffects = GetComponentsInChildren<VisualEffect>(true);
        
    }
    private void OnEnable()
    {
        Invoke(Func_DisableObject, disableTime);

        for(int i=0;i< particles.Length; i++) 
        {
            particles[i].Play();
        }

        for (int i = 0; i < visualEffects.Length; i++)
        {
            visualEffects[i].Play();
        }
    }

    private void DisableObject()
    {
        this.gameObject.SetActive(false);
    }
}