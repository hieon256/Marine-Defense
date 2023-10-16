using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[BurstCompile]
public partial struct EnemyManager : ISystem
{
    Random random;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EnemyEntity>();

        random = Unity.Mathematics.Random.CreateFromIndex(4);
    }
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        var es = SystemAPI.GetSingletonRW<EnemySpawn>();

        if (es.ValueRO.count >= 0)
        {
            Entity enemyEntity = SystemAPI.GetSingletonEntity<EnemyEntity>();
            var q = SystemAPI.GetAspect<EnemyEntityAspect>(enemyEntity);

            var copyE = q.GetEntity;

            // 화면 영역 계산.
            float3 vec = new float3(Screen.width, Screen.height, 10);
            float3 vec2 = Camera.main.ScreenToWorldPoint(vec) + (Vector3)Vector2.one * 2;
            //Vector3 vec3 = vec2 - PlayerCharacter.Instance.transform.position;

            //float A = vec3.x;
            //float B = vec3.y;
            float A = vec2.x;
            float B = vec2.y;

            float x = 0;
            float y = 0;

            for (int i = 0; i < 1; i++)
            {
                var newE = ecb.Instantiate(copyE);

                ecb.AddComponent(newE, new EnemyTag());
                ecb.AddComponent(newE, new ObjectInfo { objectType = ObjectType.Enemy, objectIndex = 0 });
                ecb.AddComponent(newE, new HPData() { hp = 50 });
                ecb.AddComponent(newE, new SpeedData { speed = 3f });

                ecb.AddComponent(newE, new AniInfo { aniState = AniStateType.Idle, speed = 1.0f });
                ecb.AddComponent(newE, new AniTileOffset());
                ecb.AddComponent(newE, new AniEffectColor { color= new float4(0f, 0, 0f, 1.0f) });

                bool XorY = random.NextInt(0, 2) == 0 ? true : false;

                if (true)
                {

                    if (XorY)
                    {
                        x = random.NextFloat(-A, A);

                        y = random.NextInt(0, 2) == 0 ? -B : B;
                    }
                    else
                    {
                        y = random.NextFloat(-B, B);

                        x = random.NextInt(0, 2) == 0 ? -A : A;
                    }

                    //x = PlayerCharacter.Instance.transform.position.x - x;
                    //y = PlayerCharacter.Instance.transform.position.y - y;
                }

                ecb.SetComponent(newE, new LocalTransform
                {
                    Position = new float3(x, y, 0),
                    Rotation = quaternion.Euler(float3.zero),
                    Scale = 1
                }); ;
                //Array.Resize(ref matArray, matArray.Length + 1);
                //matArray.SetValue(Instantiate(mat), matArray.Length);
                //ecb.AddComponent(newE, new MaterialMeshInfo { Material = 6 });
            }

            es.ValueRW.count = 1;
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
