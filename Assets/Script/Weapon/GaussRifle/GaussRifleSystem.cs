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

        // ���콺 L ��ư Ȯ��, ��Ÿ�� ���Ҵ��� Ȯ��.
        if (mouseL.Rpressed && mouseL.Lpressed && coolTime.time <= 0)
        {

            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            var bulletPrefab = SystemAPI.GetComponentRO<GaussBulletPrefab>(gaussRifle).ValueRO.entity;

            var newBullet = ecb.Instantiate(bulletPrefab);

            // �±� �߰�.
            ecb.AddComponent<GaussBulletTag>(newBullet);

            // ��ġ ����.
            playerPos.Position.z = 0;

            ecb.SetComponent(newBullet, playerPos);

            // ���� �߰�.
            var mousePos = mouseL.mousePos;
            mousePos.z = 0;

            var speed = SystemAPI.GetComponent<GaussRifleStats>(gaussRifle).speed;

            var targetDir = math.normalize(mousePos - playerPos.Position);

            ecb.SetComponent(newBullet, new PhysicsVelocity { Linear = targetDir * speed });

            // effect ����.
            effectRot.rotation = Quaternion.FromToRotation(Vector3.right, targetDir);
            effect.Play();

            // ���ӽð� �߰�.
            ecb.AddComponent(newBullet, new DurationData { duration = 0.5f });

            // ��Ÿ�� ����.
            ecb.SetComponent(gaussRifle, new GaussRifleCoolTime { time = coolTime.coolTime, coolTime = coolTime.coolTime });

            ecb.SetEnabled(newBullet, true);

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
        else
        {
            effect.Stop();
        }

        // ��Ÿ�� ����.
        if (coolTime.time > 0)
            SystemAPI.SetSingleton<GaussRifleCoolTime>(new GaussRifleCoolTime { time = coolTime.time - deltaTime, coolTime = coolTime.coolTime });

    }
}