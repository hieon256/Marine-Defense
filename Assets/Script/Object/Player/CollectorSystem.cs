using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct CollectorSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

        var player = SystemAPI.GetSingletonEntity<PlayerTag>();
        var playerPos = SystemAPI.GetComponent<LocalTransform>(player).Position;

        foreach (var i in SystemAPI.Query<CollectorAspect>())
        {
            float time = i.itemCollecting.ValueRO.time;
            float limitTime = 0.3f;

            if (time >= limitTime)
            {
                // 아이템 인벤토리에 추가.
                ecb.AddComponent(ecb.CreateEntity(), new ItemChangeData 
                {
                    fromEntity = Entity.Null,
                    fromIndex = -1,
                    toEntity = player, // player inventory id = 0.
                    toIndex = -1,
                    item = i.objInfo.ValueRO.objectIndex
                });

                i.transform.ValueRW.Position = playerPos;
                i.transform.ValueRW.Scale = 0f;

                ecb.AddComponent(i.Entity, new DestroyedEntity { });
                ecb.RemoveComponent<ItemCollecting>(i.Entity);
                continue;
            }
            else
            {
                float3 prevPos = i.itemCollecting.ValueRO.prevPos;

                i.transform.ValueRW.Position = prevPos + (playerPos - prevPos) * time / limitTime;
                i.transform.ValueRW.Scale = -time / limitTime + 1f;

                i.itemCollecting.ValueRW.time += SystemAPI.Time.DeltaTime;
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}