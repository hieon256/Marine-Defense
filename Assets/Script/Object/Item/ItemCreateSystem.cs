using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public partial struct ItemCreateSystem : ISystem
{
    Random random;
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ItemEntity>();

        random = Unity.Mathematics.Random.CreateFromIndex(4);
    }
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        var es = SystemAPI.GetSingletonRW<ItemSpawn>();

        if (es.ValueRO.count == 0)
        {
            ItemEntity itemEntity = SystemAPI.GetSingleton<ItemEntity>();

            var copyE = itemEntity.entity;

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

            for (int i = 0; i < 10; i++)
            {
                var newE = ecb.Instantiate(copyE);

                ecb.AddComponent(newE, new ItemTag());
                ecb.AddComponent(newE, new ObjectInfo { objectType = ObjectType.Item, objectIndex = 0 });

                ecb.AddComponent(newE, new AniInfo { aniState = AniStateType.Idle, speed = 1.0f });
                ecb.AddComponent(newE, new AniTileOffset());
                ecb.AddComponent(newE, new AniEffectColor { color = new float4(0f, 0, 0f, 1.0f) });

                bool XorY = random.NextInt(0, 2) == 0 ? true : false;

                if (true)
                {

                    if (XorY)
                    {
                        x = random.NextFloat(-A / 3f, A / 3f);

                        y = random.NextInt(0, 2) == 0 ? -B / 3f : B / 3f;
                    }
                    else
                    {
                        y = random.NextFloat(-B / 3f, B / 3f);

                        x = random.NextInt(0, 2) == 0 ? -A / 3f : A / 3f;
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
}
