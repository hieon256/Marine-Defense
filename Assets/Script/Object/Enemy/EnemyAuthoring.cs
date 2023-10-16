using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public class EnemyAuthoring : MonoBehaviour
{
    public Material mat;
    public MeshRenderer meshRenderer;
    public GameObject go;

    public void MaterialInit()
    {
        meshRenderer.material = Instantiate(mat);
    }
}
public class EnemyBaker : Baker<EnemyAuthoring>
{
    public override void Bake(EnemyAuthoring authoring)
    {
        authoring.MaterialInit();

        Entity entity = GetEntity(TransformUsageFlags.None);
        Entity enemyPrefab = GetEntity(authoring.go, TransformUsageFlags.None);

        EnemyEntity ee = default;
        ee.entity = enemyPrefab;

        AddComponent(entity, ee);

        EnemySpawn es = default;
        es.count = 0;

        AddComponent(entity, es);
    }
}

public struct EnemyEntity : IComponentData
{
    public Entity entity;
}
public struct EnemySpawn : IComponentData
{
    public int count;
}



public struct EnemyTag : IComponentData { }