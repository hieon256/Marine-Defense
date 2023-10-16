using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[BurstCompile]
[RequireComponent(typeof(GaussRifleTag))]
[UpdateInGroup(typeof(WeaponCustomGroup))]
public partial class GaussRifleSystem : SystemBase
{
    public Transform effectRot;
    public ParticleSystem effect;

    private bool isEnable;
    [BurstCompile]
    protected override void OnCreate()
    {
    }
    public bool Init(Transform rot, ParticleSystem ps)
    {
        effectRot = rot;
        effect = ps;
        isEnable = true;
        return true;
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        if (!isEnable)
            return;

        float deltaTime = SystemAPI.Time.DeltaTime;

        Entity player = SystemAPI.GetSingletonEntity<PlayerTag>();
        PlayerMouse mouseL = SystemAPI.GetComponent<PlayerMouse>(player);

        Entity gaussRifle = SystemAPI.GetSingletonEntity<GaussRifleTag>();
        GaussRifleCoolTime coolTime = SystemAPI.GetComponent<GaussRifleCoolTime>(gaussRifle);

        var playerPos = SystemAPI.GetComponent<LocalTransform>(player);

        effectRot.position = playerPos.Position + new float3(0, 0.02f, 0);

        // 마우스 L 버튼 확인, 쿨타임 돌았는지 확인.
        if (mouseL.Rpressed && mouseL.Lpressed && coolTime.time <= 0)
        {

            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            var bulletPrefab = SystemAPI.GetComponentRO<GaussBulletPrefab>(gaussRifle).ValueRO.entity;

            var newBullet = ecb.Instantiate(bulletPrefab);

            // 태그 추가.
            ecb.AddComponent<GaussBulletTag>(newBullet);

            // 위치 설정.
            playerPos.Position.z = 0;

            ecb.SetComponent(newBullet, playerPos);

            // 방향 추가.
            var mousePos = mouseL.mousePos;
            mousePos.z = 0;

            var speed = SystemAPI.GetComponent<GaussRifleStats>(gaussRifle).speed;

            var targetDir = math.normalize(mousePos - playerPos.Position);

            ecb.SetComponent(newBullet, new PhysicsVelocity { Linear = targetDir * speed });

            // effect 방향.
            effectRot.rotation = Quaternion.FromToRotation(Vector3.right, targetDir);
            effect.Play();

            // 지속시간 추가.
            ecb.AddComponent(newBullet, new DurationData { duration = 0.5f });

            // 쿨타임 적용.
            ecb.SetComponent(gaussRifle, new GaussRifleCoolTime { time = coolTime.coolTime, coolTime = coolTime.coolTime });

            ecb.SetEnabled(newBullet, true);

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
        else
        {
            effect.Stop();
        }

        // 쿨타임 돌기.
        if (coolTime.time > 0)
            SystemAPI.SetSingleton<GaussRifleCoolTime>(new GaussRifleCoolTime { time = coolTime.time - deltaTime, coolTime = coolTime.coolTime });

    }
}