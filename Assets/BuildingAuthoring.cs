using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BuildingAuthoring : MonoBehaviour
{
    public GameObject playerBuilding3x3;

    public GameObject enemyBuilding2x2;

    public GameObject bluePrintGO;
}

public class BuildingBaker : Baker<BuildingAuthoring>
{
    public override void Bake(BuildingAuthoring authoring)
    {
        Entity self = GetEntity(TransformUsageFlags.None);

        AddComponent(self, new BuildingSpawn { count = 0 });

        AddComponent(self, new PlayerBuildingEntity
        {
            building3x3 = GetEntity(authoring.playerBuilding3x3, TransformUsageFlags.None)
        });

        AddComponent(self, new EnemyBuildingEntity
        {
            building2x2 = GetEntity(authoring.enemyBuilding2x2, TransformUsageFlags.None)
        });

        Entity blueprint = GetEntity(authoring.bluePrintGO, TransformUsageFlags.None);
        AddComponent(self, new BluePrintEntity
        {
            entity = blueprint
        });
    }
}
public struct PlayerBuildingTag : IComponentData { }
public struct EnemyBuildingTag : IComponentData { }
public struct PlayerBuildingEntity : IComponentData
{
    public Entity building3x3;
}
public struct EnemyBuildingEntity : IComponentData
{
    public Entity building2x2;
}
public struct BluePrintEntity : IComponentData
{
    public Entity entity;
}
public struct BuildingSpawn : IComponentData
{
    public int count;
}