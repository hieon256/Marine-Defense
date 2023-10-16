using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct CollectorAspect : IAspect
{
    public readonly Entity Entity;

    public readonly RefRW<ItemCollecting> itemCollecting;
    public readonly RefRW<LocalTransform> transform;
    public readonly RefRO<ObjectInfo> objInfo;
}
