using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ItemAuthoring : MonoBehaviour
{
    public GameObject itemTemplate;
}
public class ItemBaker : Baker<ItemAuthoring>
{
    public override void Bake(ItemAuthoring authoring)
    {
        Entity self = GetEntity(TransformUsageFlags.None);
        AddComponent(self, new ItemEntity 
        { 
            entity = GetEntity(authoring.itemTemplate,TransformUsageFlags.None) 
        });
        AddComponent(self, new ItemSpawn { count = 0 });
    }
}

public struct ItemEntity : IComponentData
{
    public Entity entity;
}
public struct ItemSpawn : IComponentData
{
    public int count;
}


// item instance components
public struct ItemTag : IComponentData
{
}
public struct ItemCollecting : IComponentData
{
    public float3 prevPos;
    public float time;
}