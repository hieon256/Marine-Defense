using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    private static ResourceManager instance;
    public static ResourceManager Instance
    {
        get
        {
            return instance;
        }
    }
    private void Awake()
    {
        instance = GetComponent<ResourceManager>();
    }

    public Sprite errorImage;
    public List<Sprite> itemImages = new List<Sprite>();

    public Sprite GetItemImage(int itemIndex)
    {
        if(itemImages.Count > itemIndex && itemImages[itemIndex] != null) 
            return itemImages[itemIndex];

        return errorImage;
    }
    public bool IsBuildingType(int itemType)
    {
        return itemType >= 0 && itemType < 200;
    }
    public int GetBuildingObjType(int itemType)
    {
        return 0;
    }
    public Vector2Int GetSizeOfBuilding(int itemType)
    {
        return new Vector2Int(3, 3);
    }
    public int GetTypeOfBuilding(int itemType)
    {
        return 0;
    }
}
