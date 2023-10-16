using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
    public PlayerTypeIndex characterIndex;
    public float speed;
    public float sightRange;
    public int invenLength;
}
public class PlayerBaker : Baker<PlayerAuthoring>
{
    public override void Bake(PlayerAuthoring authoring)
    {
        Entity self = GetEntity(TransformUsageFlags.None);
        AddComponent(self, new PlayerTag { });
        AddComponent(self, new PlayerMoveInput { });
        AddComponent(self, new PlayerMouse { });

        DynamicBuffer<InventoryData> buffer = AddBuffer<InventoryData>(self);

        for(int i = 0; i < authoring.invenLength; i++) { buffer.Add(new InventoryData { item = -1 }); }

        AddComponent(self, new HPData { hp = 100 });
        AddComponent(self, new SpeedData { speed = authoring.speed });

        AddComponent(self, new SightRangeData { range = authoring.sightRange });

        // animation.
        AddComponent(self, new ObjectInfo { objectType = ObjectType.Player, objectIndex = (int)authoring.characterIndex });
        AddComponent(self, new AniInfo { aniState = AniStateType.Idle, aniDir = AniDirection.Down, speed = 1.0f });
        AddComponent(self, new AniTileOffset());
        AddComponent(self, new AniEffectColor { color = new float4(0f, 0, 0f, 1.0f) });
    }
}
public struct PlayerTag : IComponentData
{

}

public struct PlayerMoveInput : IComponentData
{
    public float2 value;
}
public struct PlayerMouse : IComponentData
{
    public bool Lpressed;
    public bool Rpressed;
    public float3 mousePos;
}