using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;

public readonly partial struct AnimationAspect : IAspect
{
    public readonly Entity Entity;
    public readonly RefRO<ObjectInfo> aniObject;

    private readonly RefRW<AniInfo> _aniInfo;
    private readonly RefRW<AniTileOffset> _aniTileOffset;
    private readonly RefRW<MaterialMeshInfo> _materialIndex;
    public AniStateType aniState
    {
        get => _aniInfo.ValueRO.aniState;
        set => _aniInfo.ValueRW.aniState = value;
    }
    public AniDirection aniDir
    {
        get => _aniInfo.ValueRO.aniDir;
    }
    public float aniPlaySpeed
    {
        get => _aniInfo.ValueRO.speed;
        set => _aniInfo.ValueRW.speed = value;
    }
    public float aniTimer
    {
        get => _aniInfo.ValueRO.time;
        set => _aniInfo.ValueRW.time = value;
    }
    public int aniOrderIndex
    {
        get => _aniInfo.ValueRO.index;
        set => _aniInfo.ValueRW.index = value;
    }
    public float4 aniTileOffset
    {
        get => _aniTileOffset.ValueRW.tileOffset;
        set => _aniTileOffset.ValueRW.tileOffset = value;
    }

    public int materialIndex
    {
        get => _materialIndex.ValueRW.Material;
        set
        {
            if(_materialIndex.ValueRW.Material != value)
                _materialIndex.ValueRW.Material = value;
        }
    }
}
