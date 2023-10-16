using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;

[UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
public partial class InputSystem : SystemBase
{
    private PlayerControl playerControl;

    private bool isClickL;
    NativeList<RaycastHit> downHits;
    NativeList<RaycastHit> upHits;
    protected override void OnCreate()
    {
        RequireForUpdate<PlayerMoveInput>();

        playerControl = new PlayerControl();
    }
    protected override void OnStartRunning()
    {
        playerControl.Enable();
    }
    protected override void OnUpdate()
    {
        var moveInput = playerControl.PlayerMap.Movement.ReadValue<Vector2>();

        SystemAPI.SetSingleton(new PlayerMoveInput { value = moveInput });

        var mouseInput = playerControl.PlayerMap.MousePos.ReadValue<Vector2>(); // ���콺 ȭ�� ��ġ ��.

        var mouseData = SystemAPI.GetSingletonRW<PlayerMouse>();
        var mousePos = Camera.main.ScreenToWorldPoint(mouseInput); // ���콺 ���� ��ġ ��.

        if (playerControl.PlayerMap.MouseL.IsPressed())
        {
            // ���� ���콺 Ŭ��.
            mouseData.ValueRW.mousePos = mousePos;
            mouseData.ValueRW.Lpressed = true;

            if (isClickL == false) // Ŭ���� ���ϰ� �ִ� ��Ȳ���� ����.
            {
                downHits = new NativeList<RaycastHit>(Allocator.Persistent); // Ŭ���� ��ƼƼ �浹��.

                PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

                var ray = new RaycastInput()
                {
                    Start = mousePos,
                    End = mousePos + Vector3.forward * 10,
                    Filter = new CollisionFilter
                    {
                        GroupIndex = 0,

                        // 1u << 0�� Physics Category Names���� 0��°�� ���̾��ũ�̴�.
                        // 1u << 1�� Physics Category Names���� 1��°�� ���̾��ũ�̴�.
                        BelongsTo = 1u << 0,
                        CollidesWith = 1u << 1
                    }
                }; // 

                physicsWorld.CastRay(ray, ref downHits);

                isClickL = true;
            }
        }
        else
        {
            mouseData.ValueRW.Lpressed = false;

            if (isClickL) // click �� �ѹ� ���� ����.
            {
                upHits = new NativeList<RaycastHit>(Allocator.Persistent); // Ŭ���ϰ� ���� �� ��ƼƼ �浹��.

                PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

                var ray = new RaycastInput()
                {
                    Start = mousePos,
                    End = mousePos + Vector3.forward * 10,
                    Filter = new CollisionFilter
                    {
                        GroupIndex = 0,

                        // 1u << 0�� Physics Category Names���� 0��°�� ���̾��ũ�̴�.
                        // 1u << 1�� Physics Category Names���� 1��°�� ���̾��ũ�̴�.
                        BelongsTo = 1u << 0,
                        CollidesWith = 1u << 1
                    }
                }; // 

                physicsWorld.CastRay(ray, ref upHits);

                NativeList<Entity> selectedEntities = new NativeList<Entity>(Allocator.Temp);

                foreach (var upHit in upHits)
                {
                    foreach (var downHit in downHits)
                    {
                        if (upHit.Entity == downHit.Entity)
                            selectedEntities.Add(upHit.Entity); // Ŭ���� ��ƼƼ�� ���� ���� ���ƾ���.
                    }
                }

                upHits.Clear();
                upHits.Dispose();
                downHits.Clear();
                downHits.Dispose();

                foreach (var entity in selectedEntities)
                {
                    Debug.Log(entity);
                }

                selectedEntities.Dispose();

                isClickL = false;
            }
        }

        if (playerControl.PlayerMap.MouseR.IsPressed())
        {
            mouseData.ValueRW.mousePos = mousePos;
            mouseData.ValueRW.Rpressed = true;
        }
        else
        {
            mouseData.ValueRW.Rpressed = false;
        }
    }
    protected override void OnStopRunning()
    {
        playerControl.Disable();
    }
}
