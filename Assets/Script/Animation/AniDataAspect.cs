using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public readonly partial struct AniFrameDataAspect : IAspect
{
    public readonly Entity Entity;

    public readonly RefRO<AniStateData> _aniState;
    private readonly RefRO<MaterialMeshInfo> _materialIndex;

    public int AniMaterialIndex
    {
        get => _materialIndex.ValueRO.Material;
    }
}