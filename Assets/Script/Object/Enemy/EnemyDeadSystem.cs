using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Rendering;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct EnemyDeadSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

        state.Dependency = new EnemyDeadJob
        {
            ecbParallel = ecb.AsParallelWriter()
        }.ScheduleParallel(state.Dependency);

        state.Dependency.Complete();

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
[BurstCompile]
public partial struct EnemyDeadJob : IJobEntity
{
    internal EntityCommandBuffer.ParallelWriter ecbParallel;
    [BurstCompile]
    private void Execute(Entity entity, [ChunkIndexInQuery] int sortKey, RefRO<HPData> enemyHp, RefRW<AniInfo> aniInfo, RefRW<PhysicsVelocity> eVel)
    {
        if(enemyHp.ValueRO.hp <= 0)
        {
            ecbParallel.AddComponent(sortKey, entity, new AniEffectColor { color = new Unity.Mathematics.float4(0, 0, 0, 1) });
            ecbParallel.RemoveComponent<PhysicsCollider>(sortKey, entity);
            ecbParallel.RemoveComponent<AniEffect>(sortKey, entity);

            eVel.ValueRW.Linear = new Unity.Mathematics.float3();
            aniInfo.ValueRW.aniState = AniStateType.Dead;
        }
    }
}