using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

public partial class CameraSystem : SystemBase
{
    public Camera compositeCamera;

    private Vector3 targetPos;
    private float targetSize;

    private const float minSize = 5f;
    public void InitCameraSystem(Camera compositeCamera)
    {
        this.compositeCamera = compositeCamera;

        Camera.main.orthographicSize = 100f;
        compositeCamera.orthographicSize = Camera.main.orthographicSize;
    }
    protected override void OnUpdate()
    {
        if (compositeCamera == null)
            return;

        targetSize = minSize; // �ּ� ũ��� ����.

        var playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
        var mouseData = SystemAPI.GetSingletonRW<PlayerMouse>();

        var playerPos = SystemAPI.GetComponent<LocalTransform>(playerEntity).Position;
        float sightMaxRange = SystemAPI.GetSingleton<SightRangeData>().range / 2f;

        float cameraSpeed = 0.05f;

        if (mouseData.ValueRO.Rpressed)
        {
            // ���� ���� ��.
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);
            float mouseMaxRange = screenSize.y / 2f * 0.9f;

            Vector2 mouseVec = (Vector2)Camera.main.WorldToScreenPoint(mouseData.ValueRO.mousePos) - screenSize / 2f;

            float currentMouseD = mouseVec.magnitude;

            if (currentMouseD >= mouseMaxRange)
                currentMouseD = mouseMaxRange;

            float ratio = Mathf.Pow(currentMouseD / mouseMaxRange, 2);

            targetSize = (sightMaxRange + 2 - minSize) * ratio + minSize;

            var target2d = playerPos.xy + (float2)mouseVec.normalized * sightMaxRange * ratio;
            targetPos = new Vector3(target2d.x, target2d.y, -10);

            cameraSpeed = 0.05f;
        }
        else
        {
            // �Ϲ� ������ ��.
            playerPos.z = -10;

            targetSize = minSize;
            targetPos = playerPos;

            cameraSpeed = 0.1f;
        }

        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetPos, cameraSpeed);
        compositeCamera.transform.position = Camera.main.transform.position;

        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetSize, cameraSpeed);
        compositeCamera.orthographicSize = Camera.main.orthographicSize;
    }
}
