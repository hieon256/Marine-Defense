using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public partial class InventorySystem : SystemBase
{
    public Action<Entity, int> removeItemAction;
    public Action<Entity, int, int> addItemAction;
    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var q in SystemAPI.Query<InventoryAspect>())
        {
            if (q.itemChangeData.ValueRO.fromEntity != Entity.Null)
            {
                DynamicBuffer<InventoryData> fromBuffer = SystemAPI.GetBuffer<InventoryData>(q.itemChangeData.ValueRO.toEntity);

                if (q.itemChangeData.ValueRO.fromIndex != -1)
                {
                    fromBuffer[q.itemChangeData.ValueRO.fromIndex] = new InventoryData { item = -1 };
                }
            }

            DynamicBuffer<InventoryData> toBuffer = SystemAPI.GetBuffer<InventoryData>(q.itemChangeData.ValueRO.toEntity);

            if (q.itemChangeData.ValueRO.toIndex == -1)
            {
                for (int i = 0; i < toBuffer.Length; i++)
                {
                    if (toBuffer[i].item == -1)
                    {
                        toBuffer[i] = new InventoryData { item = q.itemChangeData.ValueRO.item };
                        break;
                    }
                }
            }
            else if (toBuffer.Length > q.itemChangeData.ValueRO.toIndex)
                toBuffer[q.itemChangeData.ValueRO.toIndex] = new InventoryData { item = q.itemChangeData.ValueRO.item };

            ecb.DestroyEntity(q.entity);

            removeItemAction?.Invoke(q.itemChangeData.ValueRO.fromEntity, q.itemChangeData.ValueRO.fromIndex);
            addItemAction?.Invoke(q.itemChangeData.ValueRO.toEntity, q.itemChangeData.ValueRO.toIndex, q.itemChangeData.ValueRO.item);
        }

        ecb.Playback(EntityManager);
        ecb.Dispose();

        /*NativeStream.Reader reader = itemMoveStream.AsReader();

        for (int i = 0; i < reader.ForEachCount; i++)
        {
            reader.BeginForEachIndex(i);
            while (reader.RemainingItemCount > 0)
            {
                ItemChangeData itemMoveEvent = reader.Read<ItemChangeData>();

            }
            reader.EndForEachIndex();
        }*/
    }
}