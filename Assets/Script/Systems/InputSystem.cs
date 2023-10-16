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

        var mouseInput = playerControl.PlayerMap.MousePos.ReadValue<Vector2>(); // 마우스 화면 위치 값.

        var mouseData = SystemAPI.GetSingletonRW<PlayerMouse>();
        var mousePos = Camera.main.ScreenToWorldPoint(mouseInput); // 마우스 월드 위치 값.

        if (playerControl.PlayerMap.MouseL.IsPressed())
        {
            // 왼쪽 마우스 클릭.
            mouseData.ValueRW.mousePos = mousePos;
            mouseData.ValueRW.Lpressed = true;

            if (isClickL == false) // 클릭을 안하고 있는 상황에만 실행.
            {
                downHits = new NativeList<RaycastHit>(Allocator.Persistent); // 클릭한 엔티티 충돌들.

                PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

                var ray = new RaycastInput()
                {
                    Start = mousePos,
                    End = mousePos + Vector3.forward * 10,
                    Filter = new CollisionFilter
                    {
                        GroupIndex = 0,

                        // 1u << 0는 Physics Category Names에서 0번째의 레이어마스크이다.
                        // 1u << 1는 Physics Category Names에서 1번째의 레이어마스크이다.
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

            if (isClickL) // click 을 한번 했을 때만.
            {
                upHits = new NativeList<RaycastHit>(Allocator.Persistent); // 클릭하고 땠을 때 엔티티 충돌들.

                PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

                var ray = new RaycastInput()
                {
                    Start = mousePos,
                    End = mousePos + Vector3.forward * 10,
                    Filter = new CollisionFilter
                    {
                        GroupIndex = 0,

                        // 1u << 0는 Physics Category Names에서 0번째의 레이어마스크이다.
                        // 1u << 1는 Physics Category Names에서 1번째의 레이어마스크이다.
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
                            selectedEntities.Add(upHit.Entity); // 클릭한 엔티티와 땠을 때가 같아야함.
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
