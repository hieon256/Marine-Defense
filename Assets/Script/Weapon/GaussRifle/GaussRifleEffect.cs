using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEditor.PackageManager;
using UnityEngine;

public class GaussRifleEffect : MonoBehaviour
{
    public Transform bulletRot;
    public ParticleSystem bulletEffectPrefab;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadWorld());
    }
    IEnumerator LoadWorld()
    {
        while (!World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<GaussRifleSystem>().Init(bulletRot, bulletEffectPrefab))
        {
            yield return null;
        }
    }
}
