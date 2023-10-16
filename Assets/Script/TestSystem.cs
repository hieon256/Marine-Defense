using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public partial struct TestSystem : ISystem
{
    float time;
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        /*
        var deltaTime = SystemAPI.Time.DeltaTime;

        if (time >= 1f)
            time = time - 1f;

        var aniJob = new TestJob
        {
            time = time
        };

        time += deltaTime;

        state.Dependency = aniJob.ScheduleParallel(state.Dependency);
        state.Dependency.Complete(); */
    }
}

[BurstCompile]
public partial struct TestJob : IJobEntity
{
    public float time;
    [BurstCompile]
    private void Execute(RefRW<AniTileOffset> tileOffset)
    {
        if (time >= 0.75f)
            time = 0.75f;
        else if (time >= 0.5f)
            time = 0.5f;
        else if (time >= 0.25f)
            time = 0.25f;
        else
            time = 0f;

        tileOffset.ValueRW.tileOffset.z = time;
    }
}