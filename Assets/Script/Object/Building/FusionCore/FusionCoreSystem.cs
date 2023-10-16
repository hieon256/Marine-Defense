using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Debug = UnityEngine.Debug;

public partial class FusionCoreSystem : SystemBase
{
    private bool isFirst;
    private Entity fusionCoreEntity;
    protected override void OnCreate()
    {
        RequireForUpdate<PlayerTag>();
    }
    protected override void OnUpdate()
    {
        if (fusionCoreEntity == Entity.Null)
        {
            if (!isFirst)
            {
                var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

                PlayerBuildingEntity enemyBuildingE = SystemAPI.GetSingleton<PlayerBuildingEntity>();

                var copyE = enemyBuildingE.building3x3;

                var newE = ecb.Instantiate(copyE);

                ecb.AddComponent(newE, new PlayerBuildingTag());
                ecb.AddComponent(newE, new ObjectInfo { objectType = ObjectType.PlayerBuilding, objectIndex = 0 });

                ecb.AddComponent(newE, new AniInfo { aniState = AniStateType.Idle, speed = 1.0f });
                ecb.AddComponent(newE, new AniTileOffset());
                ecb.AddComponent(newE, new AniEffectColor { color = new float4(0f, 0, 0f, 1.0f) });

                ecb.AddComponent(newE, new HPData { hp = 1000 });

                ecb.SetComponent(newE, new LocalTransform
                {
                    Position = new float3(0, 0, 0),
                    Rotation = quaternion.Euler(float3.zero),
                    Scale = 1
                });

                ecb.SetEnabled(newE, true);

                ecb.Playback(EntityManager);
                ecb.Dispose();

                isFirst = true;

                return;
            }

            foreach(var a in SystemAPI.Query<PlayerBuildingAspect>())
            {
                if(a.objectInfo.ValueRO.objectIndex == 0)
                {
                    fusionCoreEntity = a.Entity;
                }
            }
        }

        var playerE = SystemAPI.GetSingletonEntity<PlayerTag>();
        RefRO<LocalTransform> playerTransform = SystemAPI.GetComponentRO<LocalTransform>(playerE);

        PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

        var pdq = new PointDistanceInput
        {
            Position = playerTransform.ValueRO.Position,
            MaxDistance = 10,
            Filter = new CollisionFilter
            {
                GroupIndex = 0,

                // 1u << 0는 Physics Category Names에서 0번째의 레이어마스크이다.
                // 1u << 1는 Physics Category Names에서 1번째의 레이어마스크이다.
                BelongsTo = 1u << 0,
                CollidesWith = 1u << 3
            }
        };

        NativeList<DistanceHit> hits = new NativeList<DistanceHit>(Allocator.Temp);

        physicsWorld.CalculateDistance(pdq, ref hits);

        foreach (DistanceHit hit in hits)
        {
            if (hit.Entity == fusionCoreEntity)
            {
                Debug.Log("퓨전 코어 : " + hit.Distance);
            }
        }
    }
}
