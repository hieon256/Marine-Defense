using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct GaussBulletSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GaussRifleTag>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (false)
        {
            // 총알 시간되면 제거.
        }
    }
}
