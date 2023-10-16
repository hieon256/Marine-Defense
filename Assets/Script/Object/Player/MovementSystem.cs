using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[BurstCompile]
[UpdateBefore(typeof(TransformSystemGroup))]
public partial struct MovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var q in SystemAPI.Query<RefRO<PlayerMoveInput>, RefRW<AniInfo>, RefRW<PhysicsVelocity>, RefRO<SpeedData>>())
        {
            float2 vel = q.Item1.ValueRO.value;

            Entity player = SystemAPI.GetSingletonEntity<PlayerTag>();
            PlayerMouse mouse = SystemAPI.GetComponent<PlayerMouse>(player);

            if (mouse.Rpressed)
            {
                // �̼� ����.
                q.Item3.ValueRW.Linear.xy = vel * q.Item4.ValueRO.speed / 2;

                // ��� ��.
                // ���� �߰�. 
                var playerPos = state.EntityManager.GetComponentData<LocalTransform>(player).Position;
                playerPos.z = 0;
                var mousePos = mouse.mousePos;
                mousePos.z = 0;
                var mouseVel = math.normalize(mousePos.xy - playerPos.xy);

                // �ִϸ��̼�
                if (math.length(vel) > 0.1f)
                {
                    // �̵� ��� ��.
                    q.Item2.ValueRW.aniState = AniStateType.Action2;
                    q.Item2.ValueRW.speed = q.Item4.ValueRO.speed;
                    q.Item2.ValueRW.aniDir = CalDirection(mouseVel);
                }
                else
                {
                    // ���� ��� ��.
                    q.Item2.ValueRW.aniState = AniStateType.Action1;
                    q.Item2.ValueRW.speed = 1.0f;
                    q.Item2.ValueRW.aniDir = CalDirection(mouseVel);
                }
            }
            else
            {
                // �̼� ����.
                q.Item3.ValueRW.Linear.xy = vel * q.Item4.ValueRO.speed;

                // �ִϸ��̼�
                if (math.length(vel) > 0.1f)
                {
                    // �̵� ��.
                    q.Item2.ValueRW.aniState = AniStateType.Move;
                    q.Item2.ValueRW.speed = q.Item4.ValueRO.speed;
                    q.Item2.ValueRW.aniDir = CalDirection(vel);
                }
                else
                {
                    // ���� ��.
                    q.Item2.ValueRW.aniState = AniStateType.Idle;
                    q.Item2.ValueRW.speed = 1.0f;
                }
            }

            break;
        }
    }
    private AniDirection CalDirection(float2 vel)
    {
        AniDirection dir = AniDirection.Up;

        if (vel.x >= 0.923)
            dir = AniDirection.Right;
        else if (vel.x <= -0.923)
            dir = AniDirection.Left;
        else
        {
            if (vel.y >= 0.923)
                dir = AniDirection.Up;
            else if (vel.y >= 0.382)
            {
                if (vel.x >= 0.382)
                    dir = AniDirection.UpRight;
                else if (vel.x <= -0.382)
                    dir = AniDirection.UpLeft;
            }
            else if (vel.y <= -0.923)
                dir = AniDirection.Down;
            else if (vel.y <= -0.382)
            {
                if (vel.x >= 0.382)
                    dir = AniDirection.DownRight;
                else if (vel.x <= -0.382)
                    dir = AniDirection.DownLeft;
            }
        }

        return dir;
    }
}
