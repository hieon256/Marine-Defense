using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateBefore(typeof(AnimationSystem))]
public partial struct EnemyMoveSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GaussRifleTag>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var player = SystemAPI.GetSingletonEntity<PlayerTag>();
        var playerPos = SystemAPI.GetComponent<LocalTransform>(player).Position;

        state.Dependency = new EnemyMoveJob()
        {
            playerPos = playerPos
        }.ScheduleParallel(state.Dependency);

        state.Dependency.Complete();
    }
    [BurstCompile]
    public partial struct EnemyMoveJob : IJobEntity
    {
        public float3 playerPos;
        [BurstCompile]
        private void Execute(EnemyTag enemyTag, RefRW<AniInfo> aniInfo, RefRW<PhysicsVelocity> eVel, RefRO<SpeedData> speed, RefRO<LocalTransform> transform)
        {
            if (aniInfo.ValueRO.aniState == AniStateType.Idle)
            {
                var vel = math.normalize(playerPos - transform.ValueRO.Position);

                eVel.ValueRW.Linear.xy = vel.xy * speed.ValueRO.speed;

                aniInfo.ValueRW.aniState = AniStateType.Move;
                aniInfo.ValueRW.speed = speed.ValueRO.speed;
                aniInfo.ValueRW.aniDir = CalDirection(vel.xy);
            }
        }
        private AniDirection CalDirection(float2 vel)
        {
            AniDirection dir = AniDirection.Up;

            if (vel.x >= 0.923)
                dir = AniDirection.Right;
            else if (vel.x <= -0.923)
                dir = AniDirection.Left;
            else
            {
                if (vel.y >= 0.923)
                    dir = AniDirection.Up;
                else if (vel.y >= 0.382)
                {
                    if (vel.x >= 0.382)
                        dir = AniDirection.UpRight;
                    else if (vel.x <= -0.382)
                        dir = AniDirection.UpLeft;
                }
                else if (vel.y <= -0.923)
                    dir = AniDirection.Down;
                else if (vel.y <= -0.382)
                {
                    if (vel.x >= 0.382)
                        dir = AniDirection.DownRight;
                    else if (vel.x <= -0.382)
                        dir = AniDirection.DownLeft;
                }
            }

            return dir;
        }
    }
}
