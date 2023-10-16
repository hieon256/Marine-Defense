using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial struct GaussBulletTrigger : ISystem
{
    ComponentLookup<GaussBulletTag> bullets;
    ComponentLookup<EnemyTag> enemies;
    ComponentLookup<EnemyBuildingTag> enemyBuildings;
    ComponentLookup<HPData> enemyHps;
    ComponentLookup<LocalTransform> enemyTransforms;

    [BurstCompile]
    public partial struct GaussBulletTriggerEvent : ICollisionEventsJob
    {
        public float gaussDamage;
        public Entity hitEffect;
        public ComponentLookup<GaussBulletTag> bullets;
        public ComponentLookup<EnemyTag> enemies;
        public ComponentLookup<EnemyBuildingTag> enemyBuildings;
        public ComponentLookup<HPData> enemyHps;
        public ComponentLookup<LocalTransform> enemyTransforms;

        public EntityCommandBuffer ecb;

        public void Execute(CollisionEvent collisionEvent)
        {
            Entity bullet = Entity.Null;
            Entity enemy = Entity.Null;

            if (bullets.HasComponent(collisionEvent.EntityA))
                bullet = collisionEvent.EntityA;
            if (bullets.HasComponent(collisionEvent.EntityB))
                bullet = collisionEvent.EntityB;
            if (enemies.HasComponent(collisionEvent.EntityA))
                enemy = collisionEvent.EntityA;
            if (enemies.HasComponent(collisionEvent.EntityB))
                enemy = collisionEvent.EntityB;
            if (enemyBuildings.HasComponent(collisionEvent.EntityA))
                enemy = collisionEvent.EntityA;
            if (enemyBuildings.HasComponent(collisionEvent.EntityB))
                enemy = collisionEvent.EntityB;

            if (Entity.Null.Equals(bullet) || Entity.Null.Equals(enemy))
                return;

            CreateHitEffect(enemyTransforms.GetRefRO(bullet).ValueRO);

            RefRW<HPData> hp = enemyHps.GetRefRW(enemy);

            float hpCal = hp.ValueRO.hp - gaussDamage;
            hp.ValueRW.hp = hpCal;
            ecb.AddComponent(enemy, new AniEffect { time = 0, effectType = 0 }); // normal effect 부여.
            ecb.AddComponent(bullet, new DestroyedEntity { });
        }
        public void CreateHitEffect(LocalTransform transform)
        {
            // hit effect 추가.
            var newHit = ecb.Instantiate(hitEffect);

            ecb.SetComponent(newHit, transform);
            ecb.AddComponent(newHit, new DurationData { duration = 0.2f });

            ecb.SetEnabled(newHit, true);
        }
    }

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        bullets = SystemAPI.GetComponentLookup<GaussBulletTag>();
        enemies = SystemAPI.GetComponentLookup<EnemyTag>();
        enemyBuildings = SystemAPI.GetComponentLookup<EnemyBuildingTag>();
        enemyHps = SystemAPI.GetComponentLookup<HPData>();
        enemyTransforms = SystemAPI.GetComponentLookup<LocalTransform>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        bullets.Update(ref state);
        enemies.Update(ref state);
        enemyBuildings.Update(ref state);
        enemyHps.Update(ref state);
        enemyTransforms.Update(ref state);

        state.Dependency = new GaussBulletTriggerEvent
        {
            gaussDamage = SystemAPI.GetSingleton<GaussRifleStats>().damage,
            hitEffect = SystemAPI.GetSingleton<GaussHitPrefab>().effectEntity,
            bullets = bullets,
            enemies = enemies,
            enemyBuildings = enemyBuildings,
            enemyHps = enemyHps,
            enemyTransforms = enemyTransforms,
            ecb = ecb
        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);

        state.Dependency.Complete();

        ecb.Playback(state.EntityManager);

        ecb.Dispose();
    }
}
