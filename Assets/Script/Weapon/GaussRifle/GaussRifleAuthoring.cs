using System.Collections;
using Unity.Entities;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class GaussRifleAuthoring : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject hitEffectPrefab;

    public float damage;

    public float coolTime;
    public float speed;

}
public class BulletBaker : Baker<GaussRifleAuthoring>
{
    public override void Bake(GaussRifleAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.None);

        AddComponent(entity, new GaussRifleTag());


        AddComponent(entity, new GaussBulletPrefab
        {
            entity = GetEntity(authoring.bulletPrefab, TransformUsageFlags.None),
        });

        AddComponent(entity, new GaussHitPrefab
        {
            effectEntity = GetEntity(authoring.hitEffectPrefab, TransformUsageFlags.None),
        });

        AddComponent(entity, new GaussRifleCoolTime
        {
            coolTime = authoring.coolTime
        });

        AddComponent(entity, new GaussRifleStats
        {
            damage = authoring.damage,
            speed = authoring.speed
        });
    }
}

public struct GaussRifleTag : IComponentData { }
public struct GaussBulletPrefab : IComponentData
{
    public Entity entity;
}
public struct GaussHitPrefab : IComponentData
{
    public Entity effectEntity;
}
public struct GaussRifleStats : IComponentData
{
    public float damage;
    public float speed;
}
public struct GaussRifleCoolTime : IComponentData
{
    public float time;
    public float coolTime;
}

