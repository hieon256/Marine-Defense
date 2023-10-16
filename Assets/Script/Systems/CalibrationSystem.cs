using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(PresentationSystemGroup), OrderFirst = true)]
public partial struct CalibrationSystem : ISystem
{
    // Update is called once per frame
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Dependency = new CalibrationJob()
        {
        }.ScheduleParallel(state.Dependency);

        state.Dependency.Complete();
    }
}

[BurstCompile]
public partial struct CalibrationJob : IJobEntity
{
    [BurstCompile]
    private void Execute(RefRW<LocalTransform> transform)
    {
        if (transform.ValueRO.Position.z != 0)
            transform.ValueRW.Position.z = 0;
    }
}