using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
public partial struct EntityDestroySystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

        state.Dependency = new EnemyDestroyJob
        {
            ecbParallel = ecb.AsParallelWriter()
        }.ScheduleParallel(state.Dependency);

        state.Dependency.Complete();

        state.Dependency = new ItemDestroyJob
        {
            ecbParallel = ecb.AsParallelWriter()
        }.ScheduleParallel(state.Dependency);

        state.Dependency.Complete();

        state.Dependency = new DurationDestroyJob
        {
            ecbParallel = ecb.AsParallelWriter(),
            deltaTime = SystemAPI.Time.DeltaTime
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
public partial struct EnemyDestroyJob : IJobEntity
{
    internal EntityCommandBuffer.ParallelWriter ecbParallel;
    [BurstCompile]
    private void Execute(Entity entity, [ChunkIndexInQuery] int sortKey, RefRO<AniInfo> aniInfo)
    {
        if (aniInfo.ValueRO.aniState == AniStateType.Dead && aniInfo.ValueRO.index == -1)
        {
            ecbParallel.DestroyEntity(sortKey, entity);
        }
    }
}
[BurstCompile]
public partial struct ItemDestroyJob : IJobEntity
{
    internal EntityCommandBuffer.ParallelWriter ecbParallel;
    [BurstCompile]
    private void Execute(Entity entity, [ChunkIndexInQuery] int sortKey, RefRO<DestroyedEntity> itemDestroyed)
    {
        ecbParallel.DestroyEntity(sortKey, entity);
    }
}
public struct DestroyedEntity : IComponentData
{

}
[BurstCompile]
public partial struct DurationDestroyJob : IJobEntity
{
    internal EntityCommandBuffer.ParallelWriter ecbParallel;
    public float deltaTime;
    [BurstCompile]
    private void Execute(Entity entity, [ChunkIndexInQuery] int sortKey, RefRW<DurationData> duration)
    {
        if(duration.ValueRO.duration <= 0)
            ecbParallel.DestroyEntity(sortKey, entity);

        duration.ValueRW.duration -= deltaTime;
    }
}