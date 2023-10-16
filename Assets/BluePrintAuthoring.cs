using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public class BluePrintAuthoring : MonoBehaviour
{
    public class BluePrintBaker : Baker<BluePrintAuthoring>
    {
        public override void Bake(BluePrintAuthoring authoring)
        {
            Entity self = GetEntity(TransformUsageFlags.None);
            AddComponent(self, new BluePrintTag());
            AddComponent(self, new BluePrintData());

            AddComponent(self, new ObjectInfo ());
            AddComponent(self, new AniInfo ());
            AddComponent(self, new AniTileOffset());
            AddComponent(self, new AniEffectColor { color = new float4(0f, 0, 0f, 1.0f) });
        }
    }
}

public struct BluePrintTag : IComponentData { }
public struct BluePrintData : IComponentData { public bool isColliding; }