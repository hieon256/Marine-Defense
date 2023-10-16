using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public readonly partial struct PlayerBuildingAspect : IAspect
{
    public readonly Entity Entity;

    public readonly RefRO<PlayerBuildingTag> playerBuildingTag;
    public readonly RefRO<ObjectInfo> objectInfo;
}
