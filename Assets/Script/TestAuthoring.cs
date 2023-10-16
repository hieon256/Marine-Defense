using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public class TestAuthoring : MonoBehaviour
{
    public int row;
    public int col;
}
public class TestBaker : Baker<TestAuthoring>
{
    public override void Bake(TestAuthoring authoring)
    {
        AddComponent(GetEntity(TransformUsageFlags.None), new AniTileOffset { tileOffset = new float4(1f/authoring.row, 1f/authoring.col, 0, 0) });
    }
}