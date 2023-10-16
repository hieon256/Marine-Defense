using System.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{

    private static BattleUIManager instance;
    public static BattleUIManager Instance
    {
        get
        {
            return instance;
        }
    }
    private void Awake()
    {
        instance = GetComponent<BattleUIManager>();

        Camera.main.transparencySortMode = TransparencySortMode.CustomAxis;
        Camera.main.transparencySortAxis = new Vector3(0, 1, 10);
        compositeCamera.transparencySortMode = TransparencySortMode.CustomAxis;
        compositeCamera.transparencySortAxis = new Vector3(0, 1, 10);
    }
    public Image startCover;

    public Camera compositeCamera;

    private EntityManager entityManager;
    private InventorySystem inventorySystem;

    private Entity bluePrintEntity;

    public BattleUIInventory playerInventory;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        StartCoroutine(GameStart());

        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        inventorySystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<InventorySystem>();
        inventorySystem.addItemAction += AddItem;
        inventorySystem.removeItemAction += RemoveItem;

        World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<CameraSystem>().InitCameraSystem(compositeCamera);

        yield return new WaitForSeconds(0.1f);

        playerInventory.InitInventory(entityManager.CreateEntityQuery(typeof(PlayerTag)).GetSingletonEntity());

        bluePrintEntity = entityManager.CreateEntityQuery(typeof(Disabled), typeof(BluePrintTag)).GetSingletonEntity();
    }
    private IEnumerator GameStart()
    {
        float t = 1.5f;
        float time = 0;

        while (startCover.color.a > 0)
        {
            Color color = startCover.color;
            color.a = -1 * Mathf.Pow(time,4) / Mathf.Pow(t,4) + 1;
            startCover.color = color;
            time += Time.deltaTime;

            yield return null;
        }
    }
    private IEnumerator LoadEntityManager()
    {
        yield return null;
    }

    public void StartDragBuildingItem(int itemType)
    {

    }
    public void DraggingBuildingItem(Vector3 eventPosition, int itemType)
    {
        if (!entityManager.IsEnabled(bluePrintEntity))
            entityManager.SetEnabled(bluePrintEntity, true);

        LocalTransform localT = entityManager.GetComponentData<LocalTransform>(bluePrintEntity); // position
        LocalToWorld localS = entityManager.GetComponentData<LocalToWorld>(bluePrintEntity); // scale

        Vector2Int size = ResourceManager.Instance.GetSizeOfBuilding(itemType); // size of building.

        Vector2 xy = Camera.main.ScreenToWorldPoint(eventPosition); // world position

        if (size.x % 2 == 0)
            xy.x = Mathf.Floor(xy.x) + 0.5f; // if x size Â¦¼ö.
        else
            xy.x = Mathf.RoundToInt(xy.x); // if x size È¦¼ö.

        if (size.y % 2 == 0)
            xy.y = Mathf.Floor(xy.y) + 0.5f;
        else
            xy.y = Mathf.RoundToInt(xy.y);

        localT.Position.x = xy.x;
        localT.Position.y = xy.y;
        localT.Position.z = 0;

        entityManager.SetComponentData(bluePrintEntity, new ObjectInfo { objectType = ObjectType.PlayerBuilding, objectIndex = ResourceManager.Instance.GetBuildingObjType(itemType) });
        entityManager.SetComponentData(bluePrintEntity, new AniInfo { aniState = AniStateType.Idle, speed = 1.0f });

        entityManager.SetComponentData(bluePrintEntity, localT);
        entityManager.AddComponentData(bluePrintEntity, new PostTransformMatrix
        {
            Value = float4x4.Scale(size.x, size.y, 1)
        });
    }
    public void EndDragBuildingItem()
    {
        if (entityManager.IsEnabled(bluePrintEntity))
            entityManager.SetEnabled(bluePrintEntity, false);
    }

    public void MoveItemByUI(Entity fromEntity, int fromIndex, Entity toEntity, int toIndex, int itemIndex)
    {
        entityManager.AddComponentData(entityManager.CreateEntity(), new ItemChangeData
        {
            fromEntity = fromEntity,
            fromIndex = fromIndex,
            toEntity = toEntity, // player inventory id = 0.
            toIndex = toIndex,
            item = itemIndex
        });
    }
    public void RemoveItem(Entity entity, int index)
    {
        if (entity == Entity.Null)
            return;

        if (entity == playerInventory.entity)
        {
            playerInventory.ItemUIChange(index, -1);
        }
    }
    public void AddItem(Entity entity, int index, int item)
    {
        if (entity == playerInventory.entity)
        {
            playerInventory.ItemUIChange(index, item);
        }
    }

    public void SelectBuildingEntity(Entity buildingEntity)
    {

    }
}
