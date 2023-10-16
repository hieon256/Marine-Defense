using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;



public enum ObjectType : byte
{
    Player,
    Enemy,
    Item,
    PlayerBuilding,
    EnemyBuilding
}
public struct ObjectInfo : IComponentData
{
    public ObjectType objectType;
    public int objectIndex;
}

#region Type Index
public enum PlayerTypeIndex : byte
{
    SpaceMarine
}
public enum EnemyTypeIndex : byte
{
    Walker
}
public enum ItemTypeIndex
{
    [InspectorName("Material/MetalPlate")]
    MetalPlate = 0,
    [InspectorName("Material/CopperWire")]
    CopperWire = 1,
    [InspectorName("Material/GasTank")]
    GasTank = 2,
    [InspectorName("Material/Firearm")]
    Firearm = 3,
    [InspectorName("Material/Paper")]
    Paper = 4,
    [InspectorName("Material/Tool")]
    Tool = 5,
    [InspectorName("Material/Lens")]
    Lens = 6,
    [InspectorName("Material/ExoticMatter")]
    ExoticMatter = 7,
    [InspectorName("Building/Barricade")]
    Barricade = 100
}
public enum PlayerBuildingTypeIndex : byte
{
    Barricade
}
public enum EnemyBuildingTypeIndex : byte
{
    Supply
}
#endregion

[InternalBufferCapacity(16)]
public struct InventoryData : IBufferElementData
{
    public int item;
}


#region Player Only
public struct SightRangeData : IComponentData
{
    public float range;
}
#endregion

#region Enemy Only
public struct EnemyCoolTime : IComponentData
{
    public float coolTime;
}
#endregion

#region Building Only
#endregion


public struct HPData : IComponentData
{
    public float hp;
}
public struct SpeedData : IComponentData
{
    public float speed;
}
public struct DurationData : IComponentData
{
    public float duration;
}