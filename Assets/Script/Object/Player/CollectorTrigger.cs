using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial struct CollectorTrigger : ISystem
{
    ComponentLookup<PlayerTag> players;
    ComponentLookup<ItemTag> items;
    ComponentLookup<LocalTransform> transforms;

    BufferLookup<InventoryData> inventories;
    [BurstCompile]
    public partial struct ItemCollectorTriggerEvent : ITriggerEventsJob
    {
        public ComponentLookup<PlayerTag> players;
        public ComponentLookup<ItemTag> items;
        public ComponentLookup<LocalTransform> transforms;

        public BufferLookup<InventoryData> inventories;

        public EntityCommandBuffer ecb;

        public void Execute(TriggerEvent collisionEvent)
        {
            Entity player = Entity.Null;
            Entity item = Entity.Null;

            if (players.HasComponent(collisionEvent.EntityA))
                player = collisionEvent.EntityA;
            if (players.HasComponent(collisionEvent.EntityB))
                player = collisionEvent.EntityB;
            if (items.HasComponent(collisionEvent.EntityA))
                item = collisionEvent.EntityA;
            if (items.HasComponent(collisionEvent.EntityB))
                item = collisionEvent.EntityB;

            if (Entity.Null.Equals(player) || Entity.Null.Equals(item))
                return;

            //플레이어 인벤토리 비었는지 확인.
            DynamicBuffer<InventoryData> inventoryBuffer = inventories[player];
            bool isNotEmpty = false;

            foreach (var itemSlot in inventoryBuffer)
            {
                if (itemSlot.item == -1)
                {
                    isNotEmpty = true;
                    break;
                }
            }

            if (!isNotEmpty)
                return;

            ecb.RemoveComponent<PhysicsCollider>(item);
            ecb.AddComponent(item, new ItemCollecting { prevPos = transforms.GetRefRO(item).ValueRO.Position });
        }
    }

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        players = SystemAPI.GetComponentLookup<PlayerTag>();
        items = SystemAPI.GetComponentLookup<ItemTag>();
        transforms = SystemAPI.GetComponentLookup<LocalTransform>();
        inventories = SystemAPI.GetBufferLookup<InventoryData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        players.Update(ref state);
        items.Update(ref state);
        transforms.Update(ref state);
        inventories.Update(ref state);



        state.Dependency = new ItemCollectorTriggerEvent
        {
            players = players,
            items = items,
            transforms = transforms,
            inventories = inventories,
            ecb = ecb
        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);

        state.Dependency.Complete();

        ecb.Playback(state.EntityManager);

        ecb.Dispose();
    }
}
