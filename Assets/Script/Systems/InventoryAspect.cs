using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public readonly partial struct InventoryAspect : IAspect
{
    public readonly Entity entity;
    public readonly RefRO<ItemChangeData> itemChangeData;
}
public struct ItemChangeData : IComponentData
{
    public Entity fromEntity;
    public int fromIndex;
    public Entity toEntity;
    public int toIndex;
    public int item;
}