using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

public readonly partial struct EnemyEntityAspect : IAspect
{
    private readonly RefRW<EnemyEntity> _enemyEntity;

    public Entity GetEntity
    {
        get => _enemyEntity.ValueRO.entity;
    }
}