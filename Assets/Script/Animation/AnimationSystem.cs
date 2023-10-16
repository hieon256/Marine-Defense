using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
public partial struct AnimationSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        Debug.Log("����");
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        NativeList<AniStateContainer> aniDatas = new NativeList<AniStateContainer>(Allocator.TempJob);

        foreach (var aniData in SystemAPI.Query<AniFrameDataAspect>())
        {
            NativeList<AniStateContainer> aniFrameDatas = new NativeList<AniStateContainer>(Allocator.TempJob);

            aniDatas.Add(new AniStateContainer
            {
                objectType = aniData._aniState.ValueRO.objectType,
                characterIndex = aniData._aniState.ValueRO.characterIndex,
                stateType = aniData._aniState.ValueRO.stateType,
                row = aniData._aniState.ValueRO.row,
                column = aniData._aniState.ValueRO.column,
                frameCount = aniData._aniState.ValueRO.frameCount,
                frameTime = aniData._aniState.ValueRO.frameTime,
                materialInfo = aniData.AniMaterialIndex
            });
        }

        var aniJob = new CharacterAniJob
        {
            deltaTime = deltaTime,
            aniDatas = aniDatas
        };

        state.Dependency = aniJob.ScheduleParallel(state.Dependency);
        state.Dependency.Complete();

        aniDatas.Dispose();

        state.Dependency = new EffectJob
        {
            deltaTime = deltaTime
        }.ScheduleParallel(state.Dependency);

        state.Dependency.Complete();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
public struct AniStateContainer
{
    public ObjectType objectType;
    public int characterIndex;
    public AniStateType stateType;

    public int row;
    public int column;

    public int frameCount;
    public float frameTime;

    public int materialInfo;
}

[BurstCompile]
public partial struct CharacterAniJob : IJobEntity
{
    public float deltaTime;
    [ReadOnly]
    public NativeList<AniStateContainer> aniDatas;
    [BurstCompile]
    private void Execute(AnimationAspect aniAspect)
    {
        ObjectType objectType = aniAspect.aniObject.ValueRO.objectType;
        int character = aniAspect.aniObject.ValueRO.objectIndex;
        // �ִϸ��̼� ���� Ȯ��. isDead, vel Ȯ�� �� ���� ����.

        foreach (AniStateContainer aniData in aniDatas)
        {
            if (aniData.objectType == objectType &&
                aniData.characterIndex == character &&
                aniData.stateType == aniAspect.aniState)
            {
                // ĳ����, �ִ� ����, ���� ��ġ.
                float timeLength = aniData.frameTime;

                float w = 1f / aniData.row;
                float h = 1f / aniData.column;

                if (aniData.column == 1)
                    aniAspect.aniTileOffset = new Unity.Mathematics.float4(w, h, w * aniAspect.aniOrderIndex, 0);
                else
                    aniAspect.aniTileOffset = new Unity.Mathematics.float4(w, h, w * aniAspect.aniOrderIndex, h * (int)aniAspect.aniDir);

                aniAspect.materialIndex = aniData.materialInfo;

                aniAspect.aniTimer += deltaTime * aniAspect.aniPlaySpeed;

                if (aniAspect.aniTimer >= timeLength)
                {
                    // ����ð� �ʰ�.
                    aniAspect.aniTimer -= timeLength;

                    if (aniAspect.aniOrderIndex == aniData.frameCount - 1)
                    {
                        // ������ ������.
                        if (aniAspect.aniState == AniStateType.Dead)
                        {
                            aniAspect.aniOrderIndex = -1; // �ı��Ҷ� �ν��ϴ� index ��.
                            return; // �׾��� ���.
                        }

                        aniAspect.aniOrderIndex = 0;

                        if (aniAspect.aniState == AniStateType.Move)
                            aniAspect.aniState = AniStateType.Idle; // ���� ���°� ������ Idle�� ��ȯ.
                    }
                    else
                    {
                        aniAspect.aniOrderIndex++;
                    }
                }
                return;
            }

        }
    }
}
[BurstCompile]
public partial struct EffectJob : IJobEntity
{
    public float deltaTime;
    [BurstCompile]
    private void Execute(EnemyEffectAspect enemyEffect)
    {
        enemyEffect.AnimateEffect(deltaTime);
    }
}